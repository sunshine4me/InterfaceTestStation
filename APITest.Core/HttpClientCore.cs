using APITest.Core.HttpHandler;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APITest.Core
{
    public class HttpClientCore : HttpClient {

        private List<string> resourceCache = new List<string>();
        public iRunLog Log { get; set; }

        //private StatisticalDataHandler statisticalDataHandler;

        public HttpClientCore(iRunLog Log) : this(Log, DefaultHabdler(Log)) {

        }

        public HttpClientCore(iRunLog Log, HttpMessageHandler handler) : base(handler) {
            this.Log = Log;
            
            base.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            base.DefaultRequestHeaders.Accept.ParseAdd("*/*");
            base.DefaultRequestHeaders.AcceptLanguage.ParseAdd("zh-CN,zh;q=0.8");
            
        }

        private static HttpMessageHandler DefaultHabdler(iRunLog Log) {
            HttpMessageHandler handler = new HttpClientHandler() { AllowAutoRedirect = false, AutomaticDecompression  = System.Net.DecompressionMethods.GZip| System.Net.DecompressionMethods.Deflate};
            handler = new MyRedirectHandler(5, handler, Log);
            //handler = new MyDecompressionHandler(System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate, handler);
            
            return handler;
        }


        public HttpResponseMessage HttpGet(Uri requestUri, Dictionary<string, string> headers, bool resourceGet = false) {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            SetRequestHeaders(request, headers);
            var res = base.SendAsync(request).Result;

            if (resourceGet) {
                try {
                    ResourceRequest(res);
                } catch (Exception) {
                   
                }
            }


            return res;
        }


        public HttpResponseMessage HttpPostForm(Uri requestUri, Dictionary<string, string> headers, IEnumerable<KeyValuePair<string, string>> formCollection, bool resourceGet = false) {
            return HttpPost(requestUri, headers, new FormUrlEncodedContent(formCollection), resourceGet);
        }

        public HttpResponseMessage HttpPost(Uri requestUri, Dictionary<string, string> headers, HttpContent body, bool resourceGet = false) {

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
          
            request.Content = body;
            SetRequestHeaders(request, headers);

            var res = base.SendAsync(request).Result;
            if (resourceGet) {
                try {
                    ResourceRequest(res);
                } catch (Exception) {

                }
            }
            return res;
        }



        private HttpRequestMessage SetRequestHeaders(HttpRequestMessage request, Dictionary<string, string> headers) {
            MediaTypeHeaderValue ct = null;
            foreach (var key in headers) {
                if (key.Key.ToLower() == "content-type") {
                    if(!MediaTypeHeaderValue.TryParse(key.Value, out ct)) {
                        Log.Warring($"setting Content-Type[{key.Value}] was failed, use default parameters Content-Type[application/x-www-form-urlencoded]");
                    }
                    continue;
                }
                request.Headers.Remove(key.Key);
                request.Headers.Add(key.Key, key.Value);
            }
            if (ct == null)
                ct = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            if(request.Content!=null)
                request.Content.Headers.ContentType = ct;

            return request;
        }



        private void ResourceRequest(HttpResponseMessage response) {

            //非html不检测资源文件
            if (response.Content.Headers.ContentType.MediaType != "text/html")
                return;

            var requestUri = response.RequestMessage.RequestUri;
            HtmlDocument hd = new HtmlDocument();
            hd.Load(response.Content.ReadAsStreamAsync().Result);

            var scripts = hd.DocumentNode.Descendants("script").ToArray();
            var links = hd.DocumentNode.Descendants("link").ToArray();
            var imgs = hd.DocumentNode.Descendants("img").ToArray();

            //提取资源文件地址
            List<Uri> ResourceUris = new List<Uri>();
            foreach (var s in scripts) {
                string src = s.GetAttributeValue("src", null);
                if (!string.IsNullOrEmpty(src)) {
                    Uri ResourceUri = new Uri(requestUri, src);
                    if (!ResourceUris.Contains(ResourceUri))
                        ResourceUris.Add(ResourceUri);
                }
            }
            foreach (var s in links) {
                string src = s.GetAttributeValue("href", null);
                if (!string.IsNullOrEmpty(src)) {
                    Uri ResourceUri = new Uri(requestUri, src);
                    if (!ResourceUris.Contains(ResourceUri))
                        ResourceUris.Add(ResourceUri);
                }
            }
            foreach (var s in imgs) {
                string src = s.GetAttributeValue("src", null);
                if (!string.IsNullOrEmpty(src)) {
                    Uri ResourceUri = new Uri(requestUri, src);
                    if (!ResourceUris.Contains(ResourceUri))
                        ResourceUris.Add(ResourceUri);
                }
            }


            List<Task> tks = new List<Task>();
            foreach (var ResourceUri in ResourceUris) {

                //是否存在缓存
                if (resourceCache.Contains(ResourceUri.AbsoluteUri)) {
                    Log.Log($"Resourc \"{ResourceUri.AbsoluteUri}\" is in the cache already and will not be download again");
                } else {
                    tks.Add(base.GetAsync(ResourceUri).ContinueWith(t => {
                        Log.Warring(t.Exception.StackTrace);
                    }, TaskContinuationOptions.OnlyOnFaulted));

                }

            }

            Task.WaitAll(tks.ToArray());
        }

    }
}
