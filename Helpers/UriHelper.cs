using Component.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Component.Helpers;

public static class UriHelper
{
    private static IHttpContextAccessor? _accessor;

    public static void ConfigureUriHelper(this IServiceProvider serviceProvider)
        => _accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

    public static Uri GetPageUri(DefaultPaginationFilter filter)
    {
        var hr = _accessor?.HttpContext?.Request;
        var baseUrl = string.Concat(hr?.Scheme, "://", hr?.Host.ToUriComponent());

        var pageUri = new Uri(string.Concat(baseUrl, hr?.Path.Value)).AbsoluteUri;
        pageUri = QueryHelpers.AddQueryString(pageUri, "page", filter.Page.ToString());
        pageUri = QueryHelpers.AddQueryString(pageUri, "per-page", filter.PerPage.ToString());
        pageUri = QueryHelpers.AddQueryString(pageUri, "first-request-time", filter.FirstRequestTime.ToString());
        if (!string.IsNullOrEmpty(filter.Sort)) pageUri = QueryHelpers.AddQueryString(pageUri, "sort", filter.Sort);
        return new Uri(pageUri);
    }
}