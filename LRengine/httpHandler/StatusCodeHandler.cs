using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LRengine.HttpHandler {
    /// <summary>
    /// 统计StatusCode 状态码总量
    /// </summary>
    public class StatusCodeHandler : DelegatingHandler {
        private ConcurrentDictionary<int, int> StatusCodeDictionary;
        public ConcurrentDictionary<int, int> GetStatusCodes() {
            return StatusCodeDictionary;
        }
        public StatusCodeHandler(HttpMessageHandler innerHandler) : base(innerHandler) {
            ResetData();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            StatusCodeDictionary.AddOrUpdate((int)response.StatusCode, 1, (key, oldValue) => oldValue + 1);
            return response;
        }

        public void ResetData() {
            StatusCodeDictionary = new ConcurrentDictionary<int, int>();
        }

        

    }

   

}
