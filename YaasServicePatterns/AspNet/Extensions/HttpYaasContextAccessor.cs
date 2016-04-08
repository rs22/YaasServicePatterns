using Microsoft.AspNet.Http;
using YaasServicePatterns.PatternSupport;

namespace YaasServicePatterns.AspNet.Extensions
{
    public class HttpYaasContextAccessor : IYaasContextAccessor {
        private readonly IHttpContextAccessor _httpContext;
        private YaasContext _yaasContext;
        
        public YaasContext YaasContext {
            get {
                if (_yaasContext == null) {
                    
                    var request = _httpContext.HttpContext.Request;
                    
                    var hybrisHop = 1; // Default value
                    int.TryParse(request.Headers["hybris-hop"], out hybrisHop);
                    
                    _yaasContext = new YaasContext {
                        HybrisHop = hybrisHop,
                        HybrisTenant = request.Headers["hybris-tenant"],
                        HybrisUser = request.Headers["hybris-user"],
                        HybrisClient = request.Headers["hybris-client"],
                        HybrisRequestID = request.Headers["hybris-request-id"],
                        HybrisScopes = request.Headers["hybris-scopes"]
                    };
                }
                return _yaasContext;
            }
        }
        
        public HttpYaasContextAccessor(IHttpContextAccessor httpContext) {
            _httpContext = httpContext;
        }
    }
}