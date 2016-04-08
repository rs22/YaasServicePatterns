using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using YaasServicePatterns.Configuration;
using YaasServicePatterns.PatternSupport;

namespace YaasServicePatterns.ServiceClients.Common {
    
    public interface IOAuthServiceClient {
        Task<YaasToken> GetTokenForTenant(string tenant);
    }
    
    public class OAuthServiceClient : IOAuthServiceClient {
        
        private readonly YaasOptions _options;
        private readonly ILogger<OAuthServiceClient> _logger;
        
        public OAuthServiceClient(IOptions<YaasOptions> options, ILogger<OAuthServiceClient> logger) {
            _options = options.Value;
            _logger = logger;
        }
        
        public async Task<YaasToken> GetTokenForTenant(string tenant) {
            var http = new HttpClient();
            var oauthTokenUri = new Uri(new Uri(_options.OAuthServiceUrl), "token");
            
            var requestedScopes = _options.DefaultScopes;

            var response = await http.PostAsync(oauthTokenUri, 
                new FormUrlEncodedContent(new [] {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"), 
                    new KeyValuePair<string, string>("client_id", _options.ClientID),
                    new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                    new KeyValuePair<string, string>("scope", $"hybris.tenant={tenant} {requestedScopes}")
                }));
                
            var result = (await ServiceResult<YaasToken>.FromResponseAsync(response)).EnsureSuccessStatusCode().Result;
            
            var grantedScopes = result.Scopes.Split(' ');
            var missingScopes = (from s in requestedScopes.Split(' ')
                                 where !grantedScopes.Contains(s)
                                 select s).ToList();
                                 
            if (missingScopes.Any()) {
                _logger.LogWarning($"The following OAuth scopes were not granted for the tenant {tenant}: {string.Join(", ", missingScopes)}");
            }
            
            return result;
        }
        
    }
}