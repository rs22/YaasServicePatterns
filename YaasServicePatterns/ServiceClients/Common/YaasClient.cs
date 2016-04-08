using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using YaasServicePatterns.PatternSupport;

namespace YaasServicePatterns.ServiceClients.Common
{
    public class YaasClient : HttpClient {
        protected YaasContext YaasContext { get; private set; }
        
        public YaasClient(IYaasContextAccessor yaasContext, IHttpContextAccessor httpContext) 
            : base(new YaasMessageHandler(new HttpClientHandler(), httpContext)) {
            YaasContext = yaasContext.YaasContext;
        }
        
        protected string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters) {
            var nameValueCollection = new NameValueCollection();
            foreach (var pair in parameters)
                nameValueCollection.Add(pair.Key, pair.Value);
                
            return BuildQueryString(nameValueCollection);
        }
        
        protected string BuildQueryString(NameValueCollection parameters) {
            var pairs = (from key in parameters.AllKeys
                         from value in parameters.GetValues(key)
                         select string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value)));
            return "?" + string.Join("&", pairs);
        }
        
        public async Task<HttpResponseMessage> FetchServiceResponseAsync(HttpMethod method, string uri, object requestData) {
            var httpRequest = new HttpRequestMessage(method, uri);
            httpRequest.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            return await SendAsync(httpRequest);
        }
        
        protected async Task<ServiceResult> ExecuteServiceRequestAsync(HttpMethod method, string uri, object requestData = null) {
            var response = await FetchServiceResponseAsync(method, uri, requestData);
            return await ServiceResult.FromResponseAsync(response);
        }
        
        protected async Task<ServiceResult<T>> ExecuteServiceRequestAsync<T>(HttpMethod method, string uri, object requestData = null) {
            var response = await FetchServiceResponseAsync(method, uri, requestData);
            return await ServiceResult<T>.FromResponseAsync(response);
        }
    }
}