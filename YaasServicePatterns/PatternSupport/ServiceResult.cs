using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YaasServicePatterns.PatternSupport
{
    public class ServiceResult {
        private readonly HttpStatusCode _statusCode;
        protected readonly string _rawResult;
        protected readonly string _errorMessage;
        
        public HttpStatusCode StatusCode { get { return _statusCode; } }
        
        public bool IsSuccessStatusCode {
            // https://github.com/dotnet/corefx/blob/master/src/System.Net.Http/src/System/Net/Http/HttpResponseMessage.cs
            get { return ((int)_statusCode >= 200) && ((int)_statusCode <= 299); }
        }
        
        public ServiceResult(HttpStatusCode statusCode, string rawResult, string errorMessage) {
            _statusCode = statusCode;
            _rawResult = rawResult;
            _errorMessage = errorMessage;
        }
        
        public ServiceResult EnsureSuccessStatusCode()
        {
            if (IsSuccessStatusCode)
            {
                return this;
            }
            
            throw new ServiceException(_statusCode, _rawResult);
        }
        
        public static async Task<ServiceResult> FromResponseAsync(HttpResponseMessage response) {
            
            string rawResult = null;
            if (response.Content != null) {
                rawResult = await response.Content.ReadAsStringAsync();    
            }
            
            return new ServiceResult(response.StatusCode, rawResult, null);
        }
        
        public static ServiceResult FromResult(HttpStatusCode statusCode) {
            return new ServiceResult(statusCode, null, null);
        }
        
        public static ServiceResult Error(HttpStatusCode statusCode, string message = null) {
            return new ServiceResult(statusCode, null, message);
        }
    }
    
    
    public class ServiceResult<T> : ServiceResult {
        private readonly T _result;
        private readonly int? _hybrisCount;
        
        public T Result { get { return _result; } }
        
        public int? HybrisCount { get { return _hybrisCount; } }
        
        public ServiceResult(HttpStatusCode statusCode, string rawResult, string errorMessage, T result, int? hybrisCount = null)
            : base(statusCode, rawResult, errorMessage) {
            _result = result;
            _hybrisCount = hybrisCount;
        }
        
        public new ServiceResult<T> EnsureSuccessStatusCode()
        {
            if (IsSuccessStatusCode)
            {
                return this;
            }
            
            throw new ServiceException(StatusCode, _rawResult);
        }
        
        public ServiceResult<TNew> WithResult<TNew>(Func<T, TNew> mapResult) {
            if (IsSuccessStatusCode) {
                return new ServiceResult<TNew>(StatusCode, _rawResult, _errorMessage, mapResult(_result), _hybrisCount);    
            } else {
                return new ServiceResult<TNew>(StatusCode, _rawResult, _errorMessage, default(TNew), _hybrisCount);
            }
        }
        
        public static new async Task<ServiceResult<T>> FromResponseAsync(HttpResponseMessage response) {
            string rawResult = null;
            if (response.Content != null) {
                rawResult = await response.Content.ReadAsStringAsync();
            }
            
            T result = default(T);
            
            if (!String.IsNullOrEmpty(rawResult)) {
                try {
                    result = JsonConvert.DeserializeObject<T>(rawResult);
                } catch {
                    // TODO: Handle json deserialization exception
                }
            }
            
            var hybrisCountHeader = response.Headers.Contains("hybris-count") ? response.Headers.GetValues("hybris-count").FirstOrDefault() : null;
            var hybrisCount = hybrisCountHeader != null ? ((int?)int.Parse(hybrisCountHeader)) : null;
            
            return new ServiceResult<T>(response.StatusCode, rawResult, null, result, hybrisCount);
        }
        
        public static ServiceResult<T> FromResult(HttpStatusCode statusCode, T result, int? hybrisCount = null) {
            return new ServiceResult<T>(statusCode, null, null, result, hybrisCount);
        }
        
        public static new ServiceResult<T> Error(HttpStatusCode statusCode, string message = null) {
            return new ServiceResult<T>(statusCode, null, message, default(T));
        }
    }
}