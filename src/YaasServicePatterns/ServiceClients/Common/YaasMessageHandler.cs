using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using YaasServicePatterns.PatternSupport;

namespace YaasServicePatterns.ServiceClients.Common
{

    public class YaasMessageHandler : DelegatingHandler {
        private readonly ILogger<YaasMessageHandler> _logger;
        private readonly YaasTokenService _tokenService;
        private readonly YaasContext _yaasContext;

        public YaasMessageHandler(HttpMessageHandler innerHandler,
                                  IHttpContextAccessor httpContext,
                                  ILogger<YaasMessageHandler> logger = null,
                                  YaasTokenService tokenService = null,
                                  IYaasContextAccessor yaasContext = null) : base(innerHandler) {

            var container = httpContext.HttpContext.RequestServices;

            _logger = logger ?? (ILogger<YaasMessageHandler>)container.GetService(typeof(ILogger<YaasMessageHandler>));
            _tokenService = tokenService ?? (YaasTokenService)container.GetService(typeof(YaasTokenService));
            var yaasContextAccessor = yaasContext ?? (IYaasContextAccessor)container.GetService(typeof(IYaasContextAccessor));
            _yaasContext = yaasContextAccessor.YaasContext;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var yaasToken = await _tokenService.ObtainTokenForTenant(_yaasContext.HybrisTenant);
            if (yaasToken != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", yaasToken.Token);

            request.Headers.Add("hybris-hop", _yaasContext.HybrisHop.ToString());
            request.Headers.Add("hybris-request-id", _yaasContext.HybrisRequestID);

            var timestamp = DateTime.Now;

            var response =  await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) {
                _logger.LogError($"Request was unsuccessful: {request.RequestUri.ToString()} {response.ReasonPhrase}");
            } else {
                _logger.LogInformation($"Request completed in {DateTime.Now - timestamp}: {request.RequestUri.ToString()}");
            }

            return response;
        }
    }
}