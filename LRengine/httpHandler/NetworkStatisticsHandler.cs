using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LRengine.httpHandler
{
    public class NetworkStatisticsHandler : DelegatingHandler {

        private int _headerFlow;
        /// <summary>
        /// header流量 byte
        /// </summary>
        public int HeaderFlow { get { return _headerFlow; } }

        private int _bodyFlow;
        /// <summary>
        /// body流量 byte
        /// </summary>
        public int BodyFlow { get { return _bodyFlow; } }
        public NetworkStatisticsHandler(HttpMessageHandler innerHandler) : base(innerHandler) {
            ResetData();
            ResetData();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            Interlocked.Add(ref _headerFlow, Encoding.UTF8.GetBytes(response.Headers.ToString()).Length);
            Interlocked.Add(ref _bodyFlow, response.Content.ReadAsByteArrayAsync().Result.Length);
            return response;
        }

        public void ResetData() {
            _headerFlow = 0;
            _bodyFlow = 0;
        }



    }
}
