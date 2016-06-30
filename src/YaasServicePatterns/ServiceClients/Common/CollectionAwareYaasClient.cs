using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using YaasServicePatterns.PatternSupport;

namespace YaasServicePatterns.ServiceClients.Common
{
    public class CollectionAwareYaasClient : YaasClient {
        public CollectionAwareYaasClient(IYaasContextAccessor yaasContext,
                                         IHttpContextAccessor httpContext) : base(yaasContext, httpContext) {}

        public async Task<ServiceResult<IList<T>>> GetItemsAsync<T>(string baseUri, DocumentsQueryParameters parameters) {
            var queryStringParams = parameters.GetUrlQueryParameters();
            return await ExecuteServiceRequestAsync<IList<T>>(HttpMethod.Get, baseUri + BuildQueryString(queryStringParams));
        }

        public async Task<ServiceResult<IList<T>>> GetAllItemsAsync<T>(string baseUri, DocumentsQueryParameters parameters) {
            var currentPage = 1;
            var results = new List<T>();
            var totalCount = int.MaxValue;

            while (results.Count < totalCount) {
                parameters.PagedAndCountable.pageNumber = currentPage++;
                parameters.PagedAndCountable.totalCount = totalCount == int.MaxValue;

                var pageResults = await GetItemsAsync<T>(baseUri, parameters);
                pageResults.EnsureSuccessStatusCode();

                results.AddRange(pageResults.Result);
                totalCount = totalCount == int.MaxValue ? pageResults.HybrisCount.Value : totalCount;
            }

            return ServiceResult<IList<T>>.FromResult(HttpStatusCode.OK, results);
        }

        public async Task<ServiceResult<T>> GetItemByIdAsync<T>(string baseUri, string id, DocumentsQueryParameters parameters) {
            parameters.Queryable.QueryParameters.Add("id", id);
            parameters.PagedAndCountable.pageNumber = 1;
            parameters.PagedAndCountable.pageSize = 1;

            var result  = await GetItemsAsync<T>(baseUri, parameters);

            if (result.Result.Count > 0) {
                return result.WithResult(x => x.FirstOrDefault());
            } else {
                return ServiceResult<T>.Error(HttpStatusCode.NotFound);
            }
        }

        public async Task<ServiceResult<ResourceLocation>> CreateItemAsync(string baseUri, object document) {
            return await ExecuteServiceRequestAsync<ResourceLocation>(HttpMethod.Post, baseUri, document);
        }

        public async Task<ServiceResult<ResourceLocation>> CreateItemWithIdAsync(string itemUri, object document) {
            return await ExecuteServiceRequestAsync<ResourceLocation>(HttpMethod.Post, itemUri, document);
        }

        public async Task<ServiceResult<T>> UpdateItemByIdAsync<T>(string baseUri, string id, object document, DocumentsQueryParameters parameters) {
            parameters.Queryable.QueryParameters.Add("id", id);

            // TODO: Some parameters are only applicable for GETs
            var queryStringParams = parameters.GetUrlQueryParameters();
            return await ExecuteServiceRequestAsync<T>(HttpMethod.Put, baseUri + BuildQueryString(queryStringParams), document);
        }

        public async Task<ServiceResult> DeleteItemByIdAsync(string baseUri, string id, DocumentsQueryParameters parameters) {
            parameters.Queryable.QueryParameters.Add("id", id);

            // TODO: Some parameters are only applicable for GETs
            var queryStringParams = parameters.GetUrlQueryParameters();
            return await ExecuteServiceRequestAsync(HttpMethod.Delete, baseUri + BuildQueryString(queryStringParams));
        }

    }
}