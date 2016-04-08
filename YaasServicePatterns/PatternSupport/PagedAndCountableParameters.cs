using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace YaasServicePatterns.PatternSupport {
    public class PagedAndCountableParameters {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public bool totalCount { get; set; }
        
        public PagedAndCountableParameters() : this(1, 0, false) {}
        
        public PagedAndCountableParameters(int pageNumber, int pageSize, bool totalCount) {
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
            this.totalCount = totalCount;
        }
        
        public IEnumerable<KeyValuePair<string, string>> GetUrlQueryParameters() {
            if (pageNumber > 0)
                yield return new KeyValuePair<string, string>("pageNumber", pageNumber.ToString());
            if (pageSize > 0)
                yield return new KeyValuePair<string, string>("pageSize", pageSize.ToString());
            if (totalCount)
                yield return new KeyValuePair<string, string>("totalCount", "true");
        }
    }
}