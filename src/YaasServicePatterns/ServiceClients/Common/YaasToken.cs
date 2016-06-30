using System;
using Newtonsoft.Json;

namespace YaasServicePatterns.ServiceClients.Common {
    public class YaasToken {
        
        [JsonProperty("access_token")]
        public string Token { get; set; }
        [JsonProperty("scope")]
        public string Scopes { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set;}

        
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsExpired => Timestamp + TimeSpan.FromSeconds(ExpiresIn) <= DateTime.Now;
    }
}