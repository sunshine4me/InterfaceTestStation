using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LRengine.httpHandler
{
    public class MyRedirectHandler : DelegatingHandler {
        private readonly int _maxAutomaticRedirections;

        public MyRedirectHandler(int maxAutomaticRedirections, HttpMessageHandler innerHandler):base(innerHandler) {

            if (maxAutomaticRedirections < 0) {
                throw new ArgumentOutOfRangeException(nameof(maxAutomaticRedirections));
            }
            _maxAutomaticRedirections = maxAutomaticRedirections;
        }

        bool RequestNeedsRedirect(HttpResponseMessage response) {
            bool needRedirect = false;
            switch (response.StatusCode) {
                case HttpStatusCode.Moved:
                case HttpStatusCode.Found:
                case HttpStatusCode.SeeOther:
                case HttpStatusCode.TemporaryRedirect:
                    needRedirect = true;
                    break;

                case HttpStatusCode.MultipleChoices:
                    needRedirect = response.Headers.Location != null; // Don't redirect if no Location specified
                    break;
            }

            return needRedirect;
        }

        private bool RequestRequiresForceGet(HttpStatusCode statusCode, HttpMethod requestMethod) {
            if (statusCode == HttpStatusCode.Moved ||
                statusCode == HttpStatusCode.Found ||
                statusCode == HttpStatusCode.SeeOther ||
                statusCode == HttpStatusCode.MultipleChoices) {
                return requestMethod == HttpMethod.Post;
            }

            return false;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            HttpResponseMessage response;
            uint redirectCount = 0;
            while (true) {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (!RequestNeedsRedirect(response)) {
                    break;
                }

                Uri location = response.Headers.Location;
                if (location == null) {
                    throw new HttpRequestException("location not found");
                }

                if (!location.IsAbsoluteUri) {
                    location = new Uri(request.RequestUri, location);
                }

                // Disallow automatic redirection from https to http
                bool allowed =
                    (request.RequestUri.Scheme == "http" && (location.Scheme == "http" || location.Scheme == "https")) ||
                    (request.RequestUri.Scheme == "https" && location.Scheme == "https");
                if (!allowed) {
                    break;
                }

                redirectCount++;
                if (redirectCount > _maxAutomaticRedirections) {
                    throw new HttpRequestException(" redirectCount Exceed maximum : " + _maxAutomaticRedirections);
                }

                // Set up for the automatic redirect
                request.RequestUri = location;

                if (RequestRequiresForceGet(response.StatusCode, request.Method)) {
                    request.Method = HttpMethod.Get;
                    request.Content = null;
                }
                //Console.WriteLine(response.Headers.ToString());
                // Do the redirect.
                response.Dispose();
            }

            return response;
        }

       
    }
}
