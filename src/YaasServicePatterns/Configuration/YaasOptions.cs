namespace YaasServicePatterns.Configuration {
    public class YaasOptions {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string OAuthServiceUrl { get; set; }
        public string DocumentServiceUrl { get; set; }
        public string ServiceID { get; set; }
        public string DefaultScopes { get; set; }
    }
}