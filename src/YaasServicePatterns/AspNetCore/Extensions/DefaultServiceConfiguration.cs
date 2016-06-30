using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using YaasServicePatterns.AspNetCore.Authorization;
using YaasServicePatterns.PatternSupport;
using YaasServicePatterns.ServiceClients.Common;
using YaasServicePatterns.ServiceClients.Document;

namespace YaasServicePatterns.AspNetCore.Extensions
{
    public static class DefaultServiceConfiguration {
        public static void AddDefaultYaasServices(this IServiceCollection services) {
            services.AddTransient<YaasClient>();
            services.AddTransient<DocumentServiceClient>();
            services.AddTransient<IYaasContextAccessor, HttpYaasContextAccessor>();
            services.AddTransient<IOAuthServiceClient, OAuthServiceClient>();
            services.AddSingleton<YaasTokenService>();
            services.AddSingleton<IAuthorizationHandler, YaasAuthorizationHandler>();
        }
    }
}