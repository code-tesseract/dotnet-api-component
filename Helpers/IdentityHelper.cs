using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Component.Helpers;

public static class IdentityHelper
{
    public static string GetClientIp(HttpContext context)
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress;
        var clientIp = string.Empty;

        if (remoteIpAddress is { AddressFamily: System.Net.Sockets.AddressFamily.InterNetworkV6 })
        {
            clientIp = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        }
        else
        {
            var clientIps = new List<string?>
            {
                context.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
                context.Connection.RemoteIpAddress?.ToString(),
                context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString()
            };

            foreach (var ip in clientIps.Where(ip => !string.IsNullOrEmpty(ip)))
            {
                clientIp = ip;
                break;
            }
        }

        return clientIp!;
    }
}