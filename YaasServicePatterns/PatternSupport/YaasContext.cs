namespace YaasServicePatterns.PatternSupport
{
    public class YaasContext {
        public string HybrisTenant { get; set; }
        
        public string HybrisUser { get; set; }
        
        public string HybrisClient { get; set; }
        
        public string HybrisRequestID { get; set; }
        
        public string HybrisScopes { get; set; }
        
        public int HybrisHop { get; set; } = 1;
    }
}