using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace YaasServicePatterns.PatternSupport {
    public class SortableParameters {
        public string sort { get; set; }
        
        public IEnumerable<KeyValuePair<string, string>> GetUrlQueryParameters() {
            if (!string.IsNullOrEmpty(sort))
                yield return new KeyValuePair<string, string>("sort", sort);   
        }
    }
}