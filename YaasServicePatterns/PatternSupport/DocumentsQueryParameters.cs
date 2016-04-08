using System.Collections.Generic;
using System.Linq;

namespace YaasServicePatterns.PatternSupport {
    public abstract class DocumentsQueryParameters {
        
        public QueryableParameters Queryable { get; set; }
        public SortableParameters Sortable { get; set; }
        public PagedAndCountableParameters PagedAndCountable { get; set; }
        public ProjectableParameters Projectable { get; set; }
        
        protected abstract QueryDefaults GetQueryDefaults();
        
        protected DocumentsQueryParameters(
            QueryableParameters queryable, 
            SortableParameters sortable, 
            PagedAndCountableParameters pagedAndCountable, 
            ProjectableParameters projectable) {
                
            // ReSharper disable once VirtualMemberCallInContructor
            var defaults = GetQueryDefaults();
            
            pagedAndCountable = pagedAndCountable ?? new PagedAndCountableParameters();
            if (pagedAndCountable.pageSize == 0)
                pagedAndCountable.pageSize = defaults.DefaultPageSize;
                
            
            projectable = string.IsNullOrEmpty(projectable?.fields) ? 
                              new ProjectableParameters(defaults.AllFields) : projectable;
            projectable.DefaultFieldNames = defaults.AllFields.ToList();
            
            Queryable = queryable ?? new QueryableParameters();
            Sortable = sortable ?? new SortableParameters();
            PagedAndCountable = pagedAndCountable;
            Projectable = projectable;
        }
        
        public IEnumerable<KeyValuePair<string, string>> GetUrlQueryParameters() {
            return Queryable.GetUrlQueryParameters()
                       .Concat(Sortable.GetUrlQueryParameters())
                       .Concat(PagedAndCountable.GetUrlQueryParameters())
                       .Concat(Projectable.GetUrlQueryParameters());
        }
        
        protected class QueryDefaults {
            public string[] QueryableFields { get; set; }
            public string[] AllFields { get; set; }
            public int DefaultPageSize { get; set; }
        }
    }
}