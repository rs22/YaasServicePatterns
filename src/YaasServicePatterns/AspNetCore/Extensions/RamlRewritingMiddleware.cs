using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace YaasServicePatterns.AspNetCore.Extensions
{
    public class RamlRewritingMiddleware {

        private readonly RequestDelegate _next;
        private readonly string _ramlUrlPrefix;

        public RamlRewritingMiddleware(RequestDelegate next, string ramlUrlPrefix)
        {
            _next = next;
            _ramlUrlPrefix = ramlUrlPrefix;
        }

        public async Task Invoke(HttpContext context)
        {
            Stream stream = null;
            MemoryStream buffer = null;

            var requestPath = context.Request.Path.Value;
            if (requestPath.StartsWith(_ramlUrlPrefix) && requestPath.EndsWith(".raml"))
            {
                // Buffer the response
                stream = context.Response.Body;
                buffer = new MemoryStream();
                context.Response.Body = buffer;
            }

            await _next.Invoke(context);

            if (buffer != null && stream != null)
            {
                context.Response.Body =  stream;

                var baseUri = context.Request.GetBaseUrl().ToString();

                buffer.Seek(0, SeekOrigin.Begin);
                using (var outputBuffer = new MemoryStream()) {
                    var reader = new StreamReader(buffer);
                    var writer = new StreamWriter(outputBuffer);

                    string line;
                    bool baseUriReplaced = false;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Transform base uri
                        if (!baseUriReplaced && line.TrimStart().StartsWith("baseUri"))
                        {
                            line = Regex.Replace(line, "^(\\s*baseUri\\s*:\\s*\"?)([^\"]*)(\"?.*)$", $"$1{baseUri}$3");
                            baseUriReplaced = true;
                        }

                        writer.WriteLine(line);
                    }

                    buffer.Dispose();
                    writer.Flush();

                    outputBuffer.Seek(0, SeekOrigin.Begin);
                    context.Response.ContentLength = outputBuffer.Length;
                    await outputBuffer.CopyToAsync(stream);
                }
            }
        }
    }

    public static class RamlRewritingMiddlewareExtensions {
        public static IApplicationBuilder UseRamlRewriterForUrlPrefix(this IApplicationBuilder builder, string ramlUrlPrefix)
        {
            return builder.UseMiddleware<RamlRewritingMiddleware>(ramlUrlPrefix);
        }
    }
}