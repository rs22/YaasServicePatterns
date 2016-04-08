using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using YaasServicePatterns.PatternSupport;

namespace YaasServicePatterns.AspNet.Mvc
{
    public class ServiceActionResult : IActionResult {
        protected IActionResult _innerResult;
        
        public ServiceActionResult(ServiceResult value, 
                                   params HttpStatusCode[] allowedStatusCodes) {
                                       
            if (!allowedStatusCodes.Contains(value.StatusCode)) {
                if (value.IsSuccessStatusCode) {
                    throw new Exception("Unexpected HTTP status code");
                } else {
                    // Throw it
                    value.EnsureSuccessStatusCode();
                }
            }
         
            _innerResult = new HttpStatusCodeResult((int)value.StatusCode);    
        }
        
        public virtual Task ExecuteResultAsync(ActionContext context) {
            return _innerResult.ExecuteResultAsync(context);
        }
    }
    
    public class ServiceActionResult<T> : ServiceActionResult {
        private readonly int? _hybrisCount;
        private readonly bool _isSuccessResult;
        
        public ServiceActionResult(ServiceResult<T> value, 
                                   params HttpStatusCode[] allowedStatusCodes)
                                   : base(value, allowedStatusCodes) {
                                       
            _hybrisCount = value.HybrisCount;
            
            if (value.IsSuccessStatusCode) {
                _isSuccessResult = true;
                
                var objectResult = new ObjectResult(value.Result);
                objectResult.StatusCode = (int)value.StatusCode;
                _innerResult = objectResult;
            } else {
                _isSuccessResult = false;
            }
        }
        
        public override Task ExecuteResultAsync(ActionContext context) {
            var includeTotalCount = false;
            if (context.HttpContext.Request.Query.ContainsKey("totalCount")) {
                bool.TryParse(context.HttpContext.Request.Query["totalCount"], out includeTotalCount);
            }
            
            if (_isSuccessResult && includeTotalCount && _hybrisCount.HasValue) {
                context.HttpContext.Response.Headers.Add("hybris-count", _hybrisCount.Value.ToString());
            }
            
            return base.ExecuteResultAsync(context);
        }
    }
}