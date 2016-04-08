using Newtonsoft.Json;

namespace YaasServicePatterns.PatternSupport {
    public class ResourceLocation {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        
        public ResourceLocation(){}
        
        public ResourceLocation(string id, string link) {
            Id = id;
            Link = link;
        }
    }
}