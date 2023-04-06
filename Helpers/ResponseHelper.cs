using System.Text.RegularExpressions;
using Component.Filters;
using Component.Models.Pagination;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

// ReSharper disable MustUseReturnValue

namespace Component.Helpers;

public static class ResponseHelper
{
    public static bool IsValidJson(this string? text)
    {
        text = text?.Trim();
        if (text != null && (!text.StartsWith("{") || !text.EndsWith("}")) &&
            (!text.StartsWith("[") || !text.EndsWith("]")))
            return false;
        try
        {
            if (text != null) JToken.Parse(text);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public static JsonSerializerSettings DefaultJsonSetting() => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new DefaultContractResolver { NamingStrategy = new DefaultNamingStrategy() }
    };

    public static async Task<string> FormatRequest(HttpRequest request)
    {
        var requestBodyString = string.Empty;
        if (request.Method.ToUpper() is not ("POST" or "PUT")) return requestBodyString;

        request.EnableBuffering();
        requestBodyString = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;
        return requestBodyString;
    }

    public static async Task<string> FormatResponse(Stream memStream)
    {
        memStream.Seek(0, SeekOrigin.Begin);
        var plainBodyText = await new StreamReader(memStream).ReadToEndAsync();
        memStream.Seek(0, SeekOrigin.Begin);

        return plainBodyText;
    }

    public static List<string> GetExceptionTraceList(Exception? ex)
    {
        var traceList = new List<string>();
        var i = 0;
        while (ex != null)
        {
            var stackTrace = ex.StackTrace;
            if (stackTrace != null)
                stackTrace = Regex.Replace(stackTrace, @"\s+", " ");

            traceList.Add($"#{i}{stackTrace}");
            ex = ex.InnerException;
            i++;
        }

        return traceList;
    }

    public static int? GetFirstErrorLineNumber(string firstTrace)
    {
        var startIndex = firstTrace.IndexOf(":line ", StringComparison.CurrentCulture) + ":line ".Length;
        var endIndex = firstTrace.LastIndexOf(" at ", StringComparison.CurrentCulture);
        if (endIndex - startIndex <= 0) return null;
        var lineString = firstTrace.Substring(startIndex, endIndex - startIndex);
        var firstLineString = lineString.Split(' ').Take(1).First();
        var isLineNumber = int.TryParse(firstLineString, out var lineResult);
        if (isLineNumber) return lineResult;
        return null;
    }

    public static string? GetFirstErrorPathFile(string firstTrace)
    {
        var startIndex = firstTrace.IndexOf("in ", StringComparison.CurrentCulture) + "in ".Length;
        var endIndex = firstTrace.LastIndexOf(":line ", StringComparison.CurrentCulture);
        return firstTrace.Substring(startIndex, endIndex - startIndex).Split(":line").Take(1)
            .DefaultIfEmpty(null)
            .First();
    }

    public static Meta GetPaginationMeta<T>(List<T> data, DefaultPaginationFilter filter, int totalRecord)
    {
        var totalPage = Convert.ToDouble(totalRecord) / Convert.ToDouble(filter.PerPage);
        var roundTotalPage = Convert.ToInt32(Math.Ceiling(totalPage));

        var record = new Record(data.Count, totalRecord);
        var page = new Page(filter.Page, roundTotalPage);

        var links = CreateLinks(filter, roundTotalPage);
        return new Meta(record, page, links);
    }

    private static Links CreateLinks(DefaultPaginationFilter filter, int totalPages)
    {
        var selfLink = UriHelper.GetPageUri(filter);
        
        var firstLink = UriHelper.GetPageUri(new DefaultPaginationFilter(1, filter.PerPage, filter.FirstRequestTime)
            { Sort = filter.Sort });
        
        var lastLink = UriHelper.GetPageUri(
            new DefaultPaginationFilter(totalPages, filter.PerPage, filter.FirstRequestTime)
                { Sort = filter.Sort });
        
        var nextLink = filter.Page >= 1 && filter.Page < totalPages
            ? UriHelper.GetPageUri(new DefaultPaginationFilter(filter.Page + 1, filter.PerPage, filter.FirstRequestTime)
                { Sort = filter.Sort })
            : null;
        
        var prevLink = filter.Page - 1 >= 1 && filter.Page <= totalPages
            ? UriHelper.GetPageUri(new DefaultPaginationFilter(filter.Page - 1, filter.PerPage, filter.FirstRequestTime)
                { Sort = filter.Sort })
            : null;

        return new Links(selfLink, firstLink, prevLink, nextLink, lastLink);
    }
}