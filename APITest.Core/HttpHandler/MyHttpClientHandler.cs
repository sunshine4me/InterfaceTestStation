using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APITest.Core.HttpHandler
{
    public class MyHttpClientHandler : HttpClientHandler {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
