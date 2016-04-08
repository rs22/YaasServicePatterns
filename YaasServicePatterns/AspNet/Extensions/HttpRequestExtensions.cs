using System;
using Microsoft.AspNet.Http;

namespace YaasServicePatterns.AspNet.Extensions
{
    internal static class HttpRequestExtensions {
        public static Uri GetBaseUrl(this HttpRequest request) {
            var hostComponents = request.Host.ToUriComponent().Split(':');

            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = hostComponents[0],
                Path = request.PathBase
            };

            if (hostComponents.Length == 2)
            {
                builder.Port = Convert.ToInt32(hostComponents[1]);
            }

            return builder.Uri;
        }
    }
}