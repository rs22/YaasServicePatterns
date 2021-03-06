using Microsoft.AspNet.Authorization;
using Microsoft.Extensions.Logging;
using YaasServicePatterns.PatternSupport;

namespace YaasServicePatterns.AspNet.Authorization {
    public class YaasAuthorizationHandler : AuthorizationHandler<YaasParameterRequirement, YaasAwareParameters> {
        
        private readonly ILogger<YaasAuthorizationHandler> _logger;
        
        public YaasAuthorizationHandler(ILogger<YaasAuthorizationHandler> logger) {
            _logger = logger;
        }
        
        protected override void Handle(AuthorizationContext context, YaasParameterRequirement requirement, YaasAwareParameters resource)
        {   
            if (requirement.IsTenantAware) {
                if (!context.User.HasClaim(x => x.Type == "HybrisTenant" && x.Value == resource.tenant)) {
                    _logger.LogInformation("Authorization failed because of invalid hybris-tenant");
                    context.Fail();
                    return;
                }
            }
            
            if (requirement.IsClientAware) {
                if (!context.User.HasClaim(x => x.Type == "HybrisClient" && x.Value == resource.client)) {
                    _logger.LogInformation("Authorization failed because of invalid hybris-client");
                    context.Fail();
                    return;
                }
            }
            
            if (requirement.IsUserAware) {
                var isRequestedUser = context.User.HasClaim(x => x.Type == "HybrisUser" && x.Value == resource.user);
                var hasFallbackScope = !string.IsNullOrEmpty(requirement.AnyUserScope) && context.User.HasClaim(x => x.Type == "HybrisScope" && x.Value == requirement.AnyUserScope);  
                
                if (!isRequestedUser && !hasFallbackScope) {
                    _logger.LogInformation("Authorization failed because of invalid hybris-user");
                    context.Fail();
                    return;
                }
            }
            
            if (!string.IsNullOrEmpty(requirement.RequiredScope)) {
                if (!context.User.HasClaim(x => x.Type == "HybrisScope" && x.Value == requirement.RequiredScope)) {
                    _logger.LogInformation("Authorization failed because of missing hybris-scope");
                    context.Fail();
                    return;
                }
            }
            
            context.Succeed(requirement);
        }
        
    }
}