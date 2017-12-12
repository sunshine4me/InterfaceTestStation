using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APITest.Core.HttpHandler
{
    /// <summary>
    /// 跳转
    /// </summary>
    public class MyRedirectHandler : DelegatingHandler {
        private readonly int _maxAutomaticRedirections;
        private iRunLog _log;
        private CookieContainer cookieContainer;
        public MyRedirectHandler(int maxAutomaticRedirections, HttpMessageHandler innerHandler, CookieContainer cookieContainer,iRunLog log):base(innerHandler) {
            this.cookieContainer = cookieContainer;
            _log = log;
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
              
                var cookieHeader = cookieContainer.GetCookieHeader(request.RequestUri);
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                _log.Network(response, cookieHeader);

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
                _log.Log($"Redirecting \"{request.RequestUri}\" (redirection depth is {redirectCount-1})");



                // Set up for the automatic redirect
                request.RequestUri = location;

                //creatNewRequest
                //var lastRequest = request;
                //request = new HttpRequestMessage(lastRequest.Method, location);
                //request.Content = lastRequest.Content;
                //foreach (var h in lastRequest.Headers) {
                //    request.Headers.Add(h.Key, h.Value);
                //}


                _log.Log($"To location \"{request.RequestUri}\"");

                if (RequestRequiresForceGet(response.StatusCode, request.Method)) {
                    request.Method = HttpMethod.Get;
                    request.Content = null;
                }
    
                // Do the redirect.
                response.Dispose();
            }

            return response;
        }


       
    }
}
