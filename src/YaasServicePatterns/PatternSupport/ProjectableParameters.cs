using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace YaasServicePatterns.PatternSupport {
    public class ProjectableParameters {
        public string fields { 
            get {
                return string.Join(",", FieldNames);
            } 
            set {
                FieldNames = value.Split(',').Select(x => x.Trim()).ToList();
            } 
        }
        
        public List<string> FieldNames { get; set; }
        public List<string> DefaultFieldNames { get; set; }
        
        public ProjectableParameters() : this(null) {}
        
        public ProjectableParameters(IEnumerable<string> fieldNames) {
            FieldNames = fieldNames?.ToList() ?? new List<string>();
        }
        
        public IEnumerable<KeyValuePair<string, string>> GetUrlQueryParameters() {
            if (!string.IsNullOrEmpty(fields)) {
                if (DefaultFieldNames == null ||
                    FieldNames.OrderBy(x => x).SequenceEqual(DefaultFieldNames.OrderBy(x => x)) == false)
                    yield return new KeyValuePair<string, string>("fields", fields);
            }
        }
    }
}