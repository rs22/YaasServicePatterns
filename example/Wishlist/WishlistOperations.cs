using YaasServicePatterns.AspNet.Authorization;

namespace Wishlist
{
    public static class WishlistOperations {
        public static YaasParameterRequirement ManageScopeRequired = 
            new YaasParameterRequirement(isTenantAware:true, requiredScope:"demo.wishlist_manage");
            
        public static YaasParameterRequirement ViewScopeRequired = 
            new YaasParameterRequirement(isTenantAware:true, requiredScope:"demo.wishlist_view");
        
    }
}