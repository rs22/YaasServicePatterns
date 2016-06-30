using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using YaasServicePatterns.Configuration;
using YaasServicePatterns.PatternSupport;
using YaasServicePatterns.ServiceClients.Common;

namespace YaasServicePatterns.ServiceClients.Document
{
    public class DocumentServiceClient : CollectionAwareYaasClient {
        private readonly YaasOptions _options;
        private readonly YaasContext _yaasContext;

        public DocumentServiceClient(IOptions<YaasOptions> options,
                                     IYaasContextAccessor yaasContext,
                                     IHttpContextAccessor httpContext) : base(yaasContext, httpContext) {
            _options = options.Value;
            _yaasContext = yaasContext.YaasContext;
        }

        private Uri GetDocumentCollectionBaseUri(string section, string collection) {
            var client = _options.ServiceID;
            return new Uri(new Uri(_options.DocumentServiceUrl), $"{_yaasContext.HybrisTenant}/{client}/{section}/{collection}/");
        }

        private Uri GetDocumentCollectionDataBaseUri(string collection) {
            return GetDocumentCollectionBaseUri("data", collection);
        }

        private Uri GetDocumentCollectionTagsBaseUri(string collection) {
            return GetDocumentCollectionBaseUri("tags", collection);
        }

        public async Task<ServiceResult<IList<T>>> GetAllDocumentsAsync<T>(string collection, DocumentsQueryParameters parameters) {
            return await GetAllItemsAsync<T>(GetDocumentCollectionDataBaseUri(collection).ToString(), parameters);
        }

        public async Task<ServiceResult<IList<T>>> GetDocumentsPagedAsync<T>(string collection, DocumentsQueryParameters parameters) {
            return await GetItemsAsync<T>(GetDocumentCollectionDataBaseUri(collection).ToString(), parameters);
        }

        public async Task<ServiceResult<T>> GetDocumentByIdAsync<T>(string collection, string id, DocumentsQueryParameters parameters) {
            return await GetItemByIdAsync<T>(GetDocumentCollectionDataBaseUri(collection).ToString(), id, parameters);
        }

        public async Task<ServiceResult<ResourceLocation>> CreateDocumentAsync(string collection, object document) {
            return await CreateItemAsync(GetDocumentCollectionDataBaseUri(collection).ToString(), document);
        }

        public async Task<ServiceResult<ResourceLocation>> CreateDocumentWithIdAsync(string collection, string id, object document) {
            return await CreateItemWithIdAsync(new Uri(GetDocumentCollectionDataBaseUri(collection), id).ToString(), document);
        }

        public async Task<ServiceResult<T>> UpdateDocumentByIdAsync<T>(string collection, string id, object document, DocumentsQueryParameters parameters) {
            return await UpdateItemByIdAsync<T>(GetDocumentCollectionDataBaseUri(collection).ToString(), id, document, parameters);
        }

        public async Task<ServiceResult> DeleteDocumentByIdAsync(string collection, string id, DocumentsQueryParameters parameters) {
            return await DeleteItemByIdAsync(GetDocumentCollectionDataBaseUri(collection).ToString(), id, parameters);
        }

        public async Task<ServiceResult<UpsertResult>> UpsertDocumentWithIdAsync(string collection, string id, object document) {
            var uri = new Uri(GetDocumentCollectionDataBaseUri(collection), id);
            var queryStringParams = new NameValueCollection { {"upsert", "true"} };

            var result = await ExecuteServiceRequestAsync<ResourceLocation>(HttpMethod.Put, uri + BuildQueryString(queryStringParams), document);
            if (result.StatusCode == HttpStatusCode.Created) {
                return result.WithResult(x => new UpsertResult { WasCreated = true, ResourceLocation = x});
            } else {
                return result.WithResult(x => new UpsertResult { WasCreated = false });
            }
        }

        public Task<ServiceResult> SetTagForDocumentAsync(string collection, string id, string tagName, params string[] tagValues) {
            var uri = new Uri(GetDocumentCollectionTagsBaseUri(collection), $"{id}/{tagName}");
            var queryStringParams = new NameValueCollection { {"tags", string.Join(", ", tagValues)} };
            return ExecuteServiceRequestAsync(HttpMethod.Post, uri + BuildQueryString(queryStringParams));
        }

        public Task<ServiceResult> DeleteTagForDocumentAsync(string collection, string id, string tagName, params string[] tagValues) {
            var uri = new Uri(GetDocumentCollectionTagsBaseUri(collection), $"{id}/{tagName}");
            var queryStringParams = new NameValueCollection { {"tags", string.Join(", ", tagValues)} };
            return ExecuteServiceRequestAsync(HttpMethod.Delete, uri + BuildQueryString(queryStringParams));
        }
    }

    public class UpsertResult {
        public bool WasCreated { get; set; }
        public ResourceLocation ResourceLocation { get; set; }
    }
}