using Microsoft.AspNet.Authorization;

namespace YaasServicePatterns.AspNet.Authorization {
    public class YaasParameterRequirement : IAuthorizationRequirement
    {
        public bool IsTenantAware { get; private set; }
        public bool IsClientAware { get; private set; }
        public bool IsUserAware { get; private set; }
        public string AnyUserScope { get; private set; }
        public string RequiredScope { get; private set; }
        
        public YaasParameterRequirement(bool isTenantAware = false,
                                        bool isClientAware = false, 
                                        bool isUserAware = false, 
                                        string anyUserScope = null, 
                                        string requiredScope = null) {
            IsTenantAware = isTenantAware;
            IsClientAware = isClientAware;
            IsUserAware = isUserAware;
            AnyUserScope = anyUserScope;
            RequiredScope = requiredScope;
        }
        
    }
}