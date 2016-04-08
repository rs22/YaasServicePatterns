using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using YaasServicePatterns.AspNet.Mvc;
using YaasServicePatterns.PatternSupport;
using YaasServicePatterns.ServiceClients.Document;
using Wishlist.Model;

namespace Wishlist.Controllers
{
    [Route("{tenant}/wishlists")]
    public class WishlistController : Controller
    {
        private const string WishlistCollection = "wishlists";
        
        private readonly DocumentServiceClient _documentService;
        private readonly IAuthorizationService _authorization;
        
        public WishlistController(DocumentServiceClient documentService, IAuthorizationService authorization) {
            _documentService = documentService;
            _authorization = authorization;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute]YaasAwareParameters yaasParameters,
                                             [FromQuery]QueryableParameters queryable = null,
                                             [FromQuery]SortableParameters sortable = null,
                                             [FromQuery]PagedAndCountableParameters pagedAndCountable = null,
                                             [FromQuery]ProjectableParameters projectable = null) {
                                                 
            // Authorize the service 'user':
            // The hybris-tenant header has to match the tenant from the url
            // and the 'demo.wishlist_view' scope is required
            // (both is expressed through the ViewScopeRequired-Operation)
            if (!await _authorization.AuthorizeAsync(User, yaasParameters, WishlistOperations.ViewScopeRequired))
                return HttpUnauthorized();
                                        
            var parameters = new WishlistQueryParameters(queryable: queryable,
                                                         sortable: sortable, 
                                                         pagedAndCountable: pagedAndCountable,
                                                         projectable: projectable);

            var result = await _documentService.GetDocumentsPagedAsync<Model.Wishlist>(WishlistCollection, parameters);
            
            // If at some point of processing we need to be sure that we actually retrieved the documents successfully:
            // result.EnsureSuccessStatusCode();
            
            return new ServiceActionResult<IList<Model.Wishlist>>(result, HttpStatusCode.OK);
        }

        [HttpGet("{wishlistId}", Name = "WishlistLocation")]
        public async Task<IActionResult> Get([FromRoute]YaasAwareParameters yaasParameters,
                                             [FromRoute]string wishlistId) {
            if (!await _authorization.AuthorizeAsync(User, yaasParameters, WishlistOperations.ViewScopeRequired))
                return HttpUnauthorized();
            
            var result = await _documentService.GetDocumentByIdAsync<Model.Wishlist>(WishlistCollection, wishlistId, new WishlistQueryParameters());
            
            // The parameter list and the end indicates which kinds of HTTP errors should be 
            // propagated from the document service result. Anything else will cause an exception
            // to be thrown. This way all possible errors are spec'ed in the action method.
            return new ServiceActionResult<Model.Wishlist>(result, HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromRoute]YaasAwareParameters yaasParameters,
                                              [FromBody]Model.Wishlist wishlist) {
            if (!await _authorization.AuthorizeAsync(User, yaasParameters, WishlistOperations.ManageScopeRequired))
                return HttpUnauthorized();
                
            if (!ModelState.IsValid)
                return HttpBadRequest(ModelState);
                
            var result = await _documentService.CreateDocumentAsync(WishlistCollection, wishlist);
            return new ServiceActionResult<ResourceLocation>(result, HttpStatusCode.Created, HttpStatusCode.Conflict, HttpStatusCode.Unauthorized);
        }

        [HttpPut("{wishlistId}")]
        public async Task<IActionResult> Put([FromRoute]YaasAwareParameters yaasParameters,
                                             [FromRoute]string wishlistId, 
                                             [FromBody]Model.Wishlist wishlist) {
            if (!await _authorization.AuthorizeAsync(User, yaasParameters, WishlistOperations.ManageScopeRequired))
                return HttpUnauthorized();
                
            if (!ModelState.IsValid)
                return HttpBadRequest(ModelState);
                
            // We have to clear the id from the update payload, otherwise the document service complains
            wishlist.Id = null;
            
            var result = await _documentService.UpdateDocumentByIdAsync<Model.Wishlist>(WishlistCollection, wishlistId, wishlist, new WishlistQueryParameters());
            return new ServiceActionResult<Model.Wishlist>(result, HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
        }

        [HttpPut("{wishlistId}")]
        public async Task<IActionResult> Delete([FromRoute]YaasAwareParameters yaasParameters,
                                                [FromRoute]string wishlistId) {
            if (!await _authorization.AuthorizeAsync(User, yaasParameters, WishlistOperations.ManageScopeRequired))
                return HttpUnauthorized();
                
            var result = await _documentService.DeleteDocumentByIdAsync(WishlistCollection, wishlistId, new WishlistQueryParameters());
            return new ServiceActionResult(result, HttpStatusCode.NoContent, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
        }
    }
}
