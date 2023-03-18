using Component.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Component.Helpers;

public static class UriHelper
{
    private static IHttpContextAccessor _accessor = null!;

    public static void Initialize(IHttpContextAccessor accessor) => _accessor = accessor;

    public static Uri GetPageUri(DefaultPaginationFilter filter)
    {
        var hr = _accessor.HttpContext?.Request;
        var baseUrl = string.Concat(hr?.Scheme, "://", hr?.Host.ToUriComponent());

        var pageUri = new Uri(string.Concat(baseUrl, hr?.Path.Value)).AbsoluteUri;
        pageUri = QueryHelpers.AddQueryString(pageUri, "page", filter.Page.ToString());
        pageUri = QueryHelpers.AddQueryString(pageUri, "per-page", filter.PerPage.ToString());
        if (!string.IsNullOrEmpty(filter.Sort)) pageUri = QueryHelpers.AddQueryString(pageUri, "sort", filter.Sort);
        return new Uri(pageUri);
    }
}