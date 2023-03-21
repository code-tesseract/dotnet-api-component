using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Component.Helpers;
using Component.Models;
using Component.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

    public async Task<Response> ExecuteAsync()
    {
        var paginationFilter = new DefaultPaginationFilter();
        var qpType = QueryParams.GetType();

        var page = qpType.GetProperty("Page")?.GetValue(QueryParams, null);
        var perPage = qpType.GetProperty("PerPage")?.GetValue(QueryParams, null);
        var sort = qpType.GetProperty("Sort")?.GetValue(QueryParams, null);

        if (page != null) paginationFilter.Page = Convert.ToInt32(page);
        if (perPage != null) paginationFilter.PerPage = Convert.ToInt32(perPage);
        if (sort != null)
        {
            var sorts = sort.ToString()?.Split(';');
            if (sorts?.Length > 0) Query = ApplyOrderBy(Query, sorts) ?? Query;
            paginationFilter.Sort = sort.ToString();
        }

        var totalRecord = await Query.CountAsync();
        var data = await Query
            .Skip((paginationFilter.Page - 1) * paginationFilter.PerPage)
            .Take(paginationFilter.PerPage)
            .ToListAsync();

        return new Response(
            message: SuccessMessage,
            data: data,
            meta: ResponseHelper.GetPaginationMeta(data, paginationFilter, totalRecord)
        );
    }

    public async Task<(int totalRecord, List<TEntity> data)> RunAsync(CancellationToken ct = default)
    {
        var paginationFilter = new DefaultPaginationFilter();
        var qpType = QueryParams.GetType();

        var page = qpType.GetProperty("Page")?.GetValue(QueryParams, null);
        var perPage = qpType.GetProperty("PerPage")?.GetValue(QueryParams, null);
        var firstRequestTime = qpType.GetProperty("FirstRequestTime")?.GetValue(QueryParams, null);
        var sort = qpType.GetProperty("Sort")?.GetValue(QueryParams, null);

        Console.WriteLine("QueryParams " + JsonConvert.SerializeObject(QueryParams));
        Console.WriteLine("FirstRequestTime " + firstRequestTime);
        if (page != null) paginationFilter.Page = Convert.ToInt32(page);
        if (perPage != null) paginationFilter.PerPage = Convert.ToInt32(perPage);
        if (firstRequestTime != null) paginationFilter.FirstRequestTime = Convert.ToInt32(firstRequestTime);

        var createdAtProperty = typeof(TEntity).GetProperty("CreatedAt");
        if (createdAtProperty != null && createdAtProperty.PropertyType == typeof(DateTime))
        {
            var firstRequestTimeDatetime = DatetimeHelper.ToDatetimeFromUnixTimeSeconds(firstRequestTime != null
                ? Convert.ToInt32(firstRequestTime)
                : paginationFilter.FirstRequestTime);

            Query = Query.Where("CreatedAt <= @0", firstRequestTimeDatetime);
        }

        if (sort != null)
        {
            var sorts = sort.ToString()?.Split(';');
            if (sorts?.Length > 0) Query = ApplyOrderBy(Query, sorts) ?? Query;
            paginationFilter.Sort = sort.ToString();
        }

        var totalRecord = await Query.CountAsync(ct);
        var data = await Query
            .Skip((paginationFilter.Page - 1) * paginationFilter.PerPage)
            .Take(paginationFilter.PerPage)
            .ToListAsync(ct);

        return (totalRecord, data);
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