using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace YaasServicePatterns.ServiceClients.Common
{
    public class YaasTokenService {
        private readonly AsyncLock _mutex = new AsyncLock();
        
        private readonly Dictionary<string, YaasToken> _accessTokens = new Dictionary<string, YaasToken>(); 
        private readonly Dictionary<string, Task<YaasToken>> _inflightAccessTokens = new Dictionary<string, Task<YaasToken>>();
        
        private readonly IOAuthServiceClient _oauthService;
        
        public YaasTokenService(IOAuthServiceClient oauthService) {
            _oauthService = oauthService;
        }
        
        public async Task<YaasToken> ObtainTokenForTenant(string tenant) {
            Task<YaasToken> task;
            using (await _mutex.LockAsync()) {
                // Is there a token that is still valid?
                YaasToken token = null;
                
                if (_accessTokens.ContainsKey(tenant))
                    token = _accessTokens[tenant];
                
                if (token != null && token.IsExpired)
                    token = _accessTokens[tenant] = null;
                    
                if (token != null)
                    return token;
                    
                Task<YaasToken> tokenTask = null;
                if (_inflightAccessTokens.ContainsKey(tenant))
                    tokenTask = _inflightAccessTokens[tenant];
                    
                if (tokenTask == null)
                    tokenTask = _inflightAccessTokens[tenant] = UpdateTokenForTenant(tenant);
                    
                task = tokenTask;
            }
            
            return await task;
        }
        
        private async Task<YaasToken> UpdateTokenForTenant(string tenant) {
            var token = await _oauthService.GetTokenForTenant(tenant);
            
            using (await _mutex.LockAsync()) {
                _accessTokens[tenant] = token;
                _inflightAccessTokens[tenant] = null;
            }
            
            return token;
        }
        
        public async Task InvalidateTokenForTenant(string tenant) {
            using (await _mutex.LockAsync()) {
                _accessTokens[tenant] = null;
            }
        }
    }
}