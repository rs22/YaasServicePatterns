using YaasServicePatterns.PatternSupport;

namespace Wishlist.Model
{
    public sealed class WishlistQueryParameters : DocumentsQueryParameters
    {   
        public WishlistQueryParameters(
            QueryableParameters queryable = null, 
            SortableParameters sortable = null, 
            PagedAndCountableParameters pagedAndCountable = null, 
            ProjectableParameters projectable = null)
            
            : base(queryable, sortable, pagedAndCountable, projectable) {}
            
        protected override QueryDefaults GetQueryDefaults() {
            // These are used in order to determine which fields to fetch from the
            // document service. Maybe reflection on the JsonPropertyAttribute could
            // also work instead
            var wishlistFields = new [] {
                "id", "url", "owner", "title", "description", "createdAt", "items"
            };
            
            return new QueryDefaults {
                AllFields = wishlistFields,
                QueryableFields = wishlistFields,
                DefaultPageSize = 16
            };
        }
    }
}