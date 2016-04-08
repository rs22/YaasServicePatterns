using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace YaasServicePatterns.AspNet.Extensions {
    public class YaasAuthenticationMiddleware {
        
        private readonly RequestDelegate _next;

        public YaasAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public Task Invoke(HttpContext context) {
            var tenant = string.Empty;
            if (context.Request.Headers.ContainsKey("hybris-tenant"))
                tenant = context.Request.Headers["hybris-tenant"];
                
            var client = string.Empty;
            if (context.Request.Headers.ContainsKey("hybris-client"))
                client = context.Request.Headers["hybris-client"];
            
            var scopes = string.Empty;
            if (context.Request.Headers.ContainsKey("hybris-scopes"))
                scopes = context.Request.Headers["hybris-scopes"];
           
            var user = string.Empty;
            if (context.Request.Headers.ContainsKey("hybris-user"))
                user = context.Request.Headers["hybris-user"];
                
            var claims = new List<Claim>();
            
            if (!string.IsNullOrEmpty(tenant))
                claims.Add(new Claim("HybrisTenant", tenant, ClaimValueTypes.String, "YaaS"));
                
            if (!string.IsNullOrEmpty(client))
                claims.Add(new Claim("HybrisClient", client, ClaimValueTypes.String, "YaaS"));
            
            if (!string.IsNullOrEmpty(scopes)) {
                foreach (var scope in scopes.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    claims.Add(new Claim("HybrisScope", scope, ClaimValueTypes.String, "YaaS"));
            }
                
            if (!string.IsNullOrEmpty(user))
                claims.Add(new Claim("HybrisUser", user, ClaimValueTypes.String, "YaaS"));
                
            var identity = new ClaimsIdentity(claims, "YaaS");
            context.User.AddIdentity(identity);
            
            return _next(context);
        }
    }
    
    public static class YaasAuthenticationMiddlewareExtensions {
        public static IApplicationBuilder UseYaasAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<YaasAuthenticationMiddleware>();
        }
    }
}