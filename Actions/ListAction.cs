using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Component.Helpers;
using Component.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Component.Actions;

public class ListAction<TEntity>
{
    public IQueryable<TEntity> Query { get; set; }
    public object QueryParams { get; set; }
    public string? SuccessMessage { get; set; }

    public ListAction(IQueryable<TEntity> query, object queryParams, string? successMessage = null)
    {
        Query = query;
        QueryParams = queryParams;
        SuccessMessage = successMessage ?? $"The {typeof(TEntity).Name.ToLower()} list loaded successfully.";
    }

    public async Task<(int totalRecord, List<TEntity> data)> ToPagedAsync(CancellationToken ct = default)
    {
        var paginationFilter = new DefaultPaginationFilter();
        var qpType = QueryParams.GetType();

        var page = qpType.GetProperty("Page")?.GetValue(QueryParams, null);
        var perPage = qpType.GetProperty("PerPage")?.GetValue(QueryParams, null);
        var filters = qpType.GetProperty("Filters")?.GetValue(QueryParams, null);
        var firstRequestTime = qpType.GetProperty("FirstRequestTime")?.GetValue(QueryParams, null);
        var sort = qpType.GetProperty("Sort")?.GetValue(QueryParams, null);

        if (page != null) paginationFilter.Page = Convert.ToInt32(page);
        if (perPage != null) paginationFilter.PerPage = Convert.ToInt32(perPage);

        /* filters logic */
        if (filters != null)
        {
            paginationFilter.Filters = Convert.ToString(filters);

            var filtersString = Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToString(filters)!));
            var filtersObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(filtersString);

            var filterList = (from filter in filtersObj!
                let values = ((JArray)filter.Value).ToObject<List<object>>()
                select new FilterProperties
                {
                    Field = filter.Key,
                    Operator = values![0].ToString()!,
                    Value = values[1]
                }).ToList();

            if (filterList.Count > 0) Query = ApplyFilters(Query, filterList);
        }

        /* first request time logic */
        if (firstRequestTime != null) paginationFilter.FirstRequestTime = Convert.ToInt32(firstRequestTime);
        var createdAtProperty = typeof(TEntity).GetProperty("CreatedAt");
        if (createdAtProperty != null && createdAtProperty.PropertyType == typeof(DateTime))
        {
            var firstRequestTimeDatetime = DatetimeHelper.ToDatetimeFromUnixTimeSeconds(firstRequestTime != null
                ? Convert.ToInt32(firstRequestTime)
                : paginationFilter.FirstRequestTime);

            Query = Query.Where("CreatedAt <= @0", firstRequestTimeDatetime);
        }

        /* sort logic */
        if (sort != null)
        {
            var sorts = sort.ToString()?.Split(';');
            if (sorts?.Length > 0) Query = ApplyOrderBy(Query, sorts) ?? Query;
            paginationFilter.Sort = sort.ToString();
        }

        /* get the total record and return to list */
        var totalRecord = await Query.CountAsync(ct);
        var data = await Query
            .Skip((paginationFilter.Page - 1) * paginationFilter.PerPage)
            .Take(paginationFilter.PerPage)
            .ToListAsync(ct);

        return (totalRecord, data);
    }

    private static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, List<FilterProperties> filterList)
    {
        var filteredQuery = query;
        var entityProperties = typeof(TEntity).GetProperties();
        foreach (var filter in filterList)
        {
            var prop = entityProperties.FirstOrDefault(e => e.Name.Equals(filter.Field));
            if (prop != null)
            {
                switch (filter.Operator)
                {
                    case FilterOperators.EqualOperator:
                        filteredQuery = filteredQuery.Where($"{filter.Field} == @0", filter.Value);
                        break;
                    case FilterOperators.NotEqualOperator:
                        filteredQuery = filteredQuery.Where($"{filter.Field} != @0", filter.Value);
                        break;
                    case FilterOperators.LikeOperator:
                        filteredQuery = filteredQuery.Where($"{filter.Field}.Contains(@0)", filter.Value);
                        break;
                    case FilterOperators.NotLikeOperator:
                        filteredQuery = filteredQuery.Where($"!{filter.Field}.Contains(@0)", filter.Value);
                        break;
                    case FilterOperators.BetweenOperator:
                    {
                        object from = null!;
                        object until = null!;
                        var allowBetween = new[]
                            {
                                typeof(DateTime),
                                typeof(double),
                                typeof(int),
                                typeof(long),
                                typeof(float),
                                typeof(decimal)
                            }
                            .Contains(prop.PropertyType);

                        if (allowBetween)
                        {
                            if (prop.PropertyType == typeof(DateTime))
                            {
                                from = DateTime.Parse(((JArray)filter.Value)[0].ToString());
                                until = DateTime.Parse(((JArray)filter.Value)[1].ToString());
                            }
                            else
                            {
                                from = Convert.ToDouble(((JArray)filter.Value)[0]);
                                until = Convert.ToDouble(((JArray)filter.Value)[1]);
                            }
                        }

                        filteredQuery = filteredQuery.Where($"{filter.Field} >= @0 AND {filter.Field} <= @1", from,
                            until);
                        break;
                    }
                    case FilterOperators.LessThanOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{filter.Field} < @0", filter.Value);
                        break;
                    }
                    case FilterOperators.LessThanEqualOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{filter.Field} <= @0", filter.Value);
                        break;
                    }
                    case FilterOperators.GreaterThanOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{filter.Field} > @0", filter.Value);
                        break;
                    }
                    case FilterOperators.GreaterThanEqualOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{filter.Field} >= @0", filter.Value);
                        break;
                    }
                    case FilterOperators.InOperator:
                    {
                        filteredQuery = filteredQuery.Where($"{(JArray)filter.Value}.Contains(@0)", filter.Field);
                        break;
                    }
                    case FilterOperators.NotInOperator:
                    {
                        filteredQuery = filteredQuery.Where($"!{(JArray)filter.Value}.Contains(@0)", filter.Field);
                        break;
                    }
                    default: continue;
                }
            }
        }

        return filteredQuery;
    }

    private static IQueryable<T>? ApplyOrderBy<T>(IQueryable<T> query, IEnumerable<string> propertyNames)
    {
        var entityType = typeof(T);
        var parameter = Expression.Parameter(entityType, "x");
        var orderedQuery = query.OrderBy(x => 0);

        foreach (var originalPropertyName in propertyNames)
        {
            var propertyName = Regex.Replace(originalPropertyName, @"[^0-9a-zA-Z]+", "");
            var property = entityType.GetProperty(propertyName);

            if (property == null) continue;

            var isAsc = originalPropertyName[0] != '-';

            var propertyType = property.PropertyType;
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByLambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = isAsc ? "ThenBy" : "ThenByDescending";
            var orderByMethod = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName &&
                              method.IsGenericMethodDefinition &&
                              method.GetGenericArguments().Length == 2 &&
                              method.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, propertyType);

            if (orderedQuery != null)
            {
                orderedQuery = (IOrderedQueryable<T>)orderByMethod.Invoke(null, new object[]
                    { orderedQuery, orderByLambda }
                )!;
            }
        }

        return orderedQuery;
    }
}