using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace YaasServicePatterns.AspNet.Extensions
{
    public static class DefaultServiceMiddlewareExtensions {
        public static IApplicationBuilder UseDefaultServiceMiddleware(this IApplicationBuilder builder, IHostingEnvironment env, IConfigurationRoot configuration)
        {
            // Rewrite the request path to the hybris-external-url
            builder.Use(async (context, next) => {
                if (context.Request.Headers.ContainsKey("hybris-external-url")) {
                    var externalUrl = new Uri(context.Request.Headers["hybris-external-url"]);
                    context.Request.Scheme = externalUrl.Scheme;
                    context.Request.Host = new HostString(externalUrl.Host);
                    context.Request.PathBase = new PathString(externalUrl.AbsolutePath);
                    context.Items.Add("ApiProxyRequest", true);
                } else {
                    context.Items.Add("ApiProxyRequest", false);
                }
                
                await next();
            });
            
            if (!env.IsDevelopment()) {
                // Enforce HTTPS
                builder.Use(async (context, next) =>
                {
                    if (context.Request.IsHttps)
                    {
                        await next();
                        return;
                    }
                    
                    context.Response.OnStarting(() => {
                        context.Response.StatusCode = 400;
                        return Task.FromResult(0);
                    });
                });
                
                // Authenticate Requests
                builder.Use(async (context, next) =>
                {
                    string authorizationHeader = context.Request.Headers["Authorization"];
                    if (authorizationHeader != null && authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                    {
                        var token = authorizationHeader.Substring("Basic ".Length).Trim();
                        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');
                        if (credentials.Length == 2 && credentials[0] == configuration["Yaas:BasicAuthUser"]
                                                    && credentials[1] == configuration["Yaas:BasicAuthPassword"])
                        {
                            await next();
                            return;
                        }
                    }
                    
                    context.Response.OnStarting(() => {
                        context.Response.StatusCode = 403;
                        return Task.FromResult(0);
                    });
                });
            }
            
            // Redirect from the root path to either the external or internal API documentation
            builder.Use(async (context, next) =>
            {
                if (context.Request.Path == "/") {
                    var ramlRoot = "api/api.raml";
                    
                    if (context.Items.ContainsKey("ApiProxyRequest") && !(bool)context.Items["ApiProxyRequest"]) {
                        ramlRoot = "api/api.internal.raml";
                    }
                    
                    var baseUri = new Uri(context.Request.GetBaseUrl().ToString().TrimEnd('/') + "/");
                    context.Response.Redirect(new Uri(baseUri, "api-console/index.html?raml=" + ramlRoot).ToString());
                } else {
                    await next.Invoke();
                }
            });
            
            // Rewrite the base uri in RAML files
            builder.UseRamlRewriterForUrlPrefix("/api-console/api/");

            // Add a mime-type mapping for RAML files
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".raml", "text/plain");
            builder.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
            
            builder.UseYaasAuthentication();
            
            return builder;
        }
    }
}