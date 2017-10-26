using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using LRengine.extend;
using LRengine.httpHandler;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Net;
using LRengine.report;
using System.Net.Http.Headers;

namespace LRengine
{
    
    public class functionLibrary {

        

        private StatusCodeHandler statusCodeHandler;
        private NetworkStatisticsHandler networkStatisticsHandler;
        private HttpClient httpClient;
        private List<string> resourceCache = new List<string>();

        private Dictionary<string, string> customAutoHeader = new Dictionary<string, string>();
        private Dictionary<string, string> nextRequestHeader = new Dictionary<string, string>();

        

        public iRunLog log;
        public Keyword EXTRARES { get; private set; }
        public Keyword ENDITEM { get; private set; }
        public Keyword LAST { get; private set; }

        public Keyword ITEMDATA { get; private set; }
        


        public functionLibrary(iRunLog log) {
            

            HttpMessageHandler handler;
            handler = new HttpClientHandler() { AllowAutoRedirect = false };
            statusCodeHandler = new StatusCodeHandler(handler);
            handler = statusCodeHandler;
            handler = new MyRedirectHandler(5, handler);
            networkStatisticsHandler = new NetworkStatisticsHandler(handler);
            handler = new MyDecompressionHandler(System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate, networkStatisticsHandler);

         
            httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
            httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("zh-CN,zh;q=0.8");


            EXTRARES = Keyword.EXTRARES;
            ENDITEM = Keyword.ENDITEM;
            LAST = Keyword.LAST;
            ITEMDATA = Keyword.ITEMDATA;
            this.log = log;
        }





        public void web_add_header(string name, string value) {
            nextRequestHeader[name] = value;
        }

        public void web_add_auto_header(string name, string value) {
            customAutoHeader[name] = value;
            httpClient.DefaultRequestHeaders.Remove(name);
            httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public void web_remove_auto_header(string name, params object[] attrs) {
            httpClient.DefaultRequestHeaders.Remove(name);
        }

        public void web_cleanup_auto_headers() {
           foreach(var key in customAutoHeader) {
                httpClient.DefaultRequestHeaders.Remove(key.Key);
            }
        }

        

        public void web_url(string name, string url, params object[] attrs) {
            StepReport report = new StepReport();
            report.Name = name;
            var targetUri = new Uri(url.Split('=', 2)[1]);
            var request = getRequest(HttpMethod.Get, targetUri);

            //处理Referer;
            Dictionary<string, string> attributes = getAttributes(attrs);
            if (attributes.ContainsKey("referer")) {
                request.Headers.Referrer = new Uri(attributes["referer"]);
            }

            var response =  httpClient.SendAsync(request).Result;

            completeToReport(report, response, attributes);
        }

        public void web_submit_data(string name, string action, params object[] attrs) {

            StepReport report = new StepReport();
            report.Name = name;

            var targetUri = new Uri(action.Split('=', 2)[1]);
            var request = getRequest(HttpMethod.Post, targetUri);

            //处理Referer;
            Dictionary<string, string> attributes = getAttributes(attrs);
            if (attributes.ContainsKey("referer")) {
                request.Headers.Referrer = new Uri(attributes["referer"]);
            }

           

            var datas = getDataFromKey(attrs, Keyword.ITEMDATA);

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            foreach (var d in datas) {
                string k = null;
                string v = null;
                if(d.TryGetValue("Name",out k) && d.TryGetValue("Value", out v))
                    paramList.Add(new KeyValuePair<string, string>(k,v));
            }

            request.Content = new FormUrlEncodedContent(paramList);
            var response = httpClient.SendAsync(request).Result;

            completeToReport(report, response, attributes);
           

        }


        private void completeToReport(StepReport report,HttpResponseMessage response, Dictionary<string, string> attributes) {
            report.StatusCode = response.StatusCode;

            //html模式与url模式
            if (attributes.ContainsKey("mode") && attributes["mode"].ToLower() == "html") {
                var rrs = ResourceGet(response);
                //资源文件结果
                report.Resources = rrs;
            }


            //获取HttpStatusCodes 统计
            report.TotalStatusCodes = statusCodeHandler.GetStatusCodes();
            statusCodeHandler.ResetData();

            //获取流量统计
            report.HeaderFlow = networkStatisticsHandler.HeaderFlow;
            report.BodyFlow = networkStatisticsHandler.BodyFlow;
            networkStatisticsHandler.ResetData();

            log.StepLog(report);
        }

        private List<ResourceReport> ResourceGet(HttpResponseMessage response) {

            //非html不检测资源文件
            if (response.Content.Headers.ContentType.MediaType != "text/html")
                return null;

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
            List<ResourceReport> ResourceReports = new List<ResourceReport>();
            foreach (var ResourceUri in ResourceUris) {

                //是否存在缓存
                if (resourceCache.Contains(ResourceUri.AbsoluteUri)) {
                    ResourceReports.Add(new ResourceReport() { Url = ResourceUri.AbsoluteUri, isFormCache = true, StatusCode = HttpStatusCode.OK });
                } else {
                    tks.Add(httpClient.GetAsync(ResourceUri).ContinueWith(t => {
                        if (t.Result.StatusCode == HttpStatusCode.OK)
                            resourceCache.Add(ResourceUri.AbsoluteUri);
                        ResourceReports.Add(new ResourceReport() { Url = ResourceUri.AbsoluteUri, isFormCache = false, StatusCode = t.Result.StatusCode });
                    }));
                    
                }

            }

           

            Task.WaitAll(tks.ToArray());
            return ResourceReports;
        }



        //额外参数相关

        /// <summary>
        /// 创建request 并带上header
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private HttpRequestMessage getRequest(HttpMethod method, Uri requestUri) {
            var request = new HttpRequestMessage(method, requestUri);
            foreach (var key in nextRequestHeader) {
                request.Headers.Remove(key.Key);
                request.Headers.Add(key.Key, key.Value);
            }
            nextRequestHeader = new Dictionary<string, string>();
            return request;
        }

        /// <summary>
        /// Attributes 数据处理
        /// </summary>
        /// <param name="attrs"></param>
        /// <returns></returns>
        private Dictionary<string, string> getAttributes(object[] attrs) {
            var rtd = new Dictionary<string, string>();
            foreach (var attr in attrs) {
                if (attr.GetType() == typeof(string)) {
                    this.addAttr(rtd, attr.ToString());
                }
                if (attr is Keyword) break;
            }
            return rtd;
        }

        /// <summary>
        /// 获取追加数据
        /// </summary>
        /// <param name="attrs"></param>
        /// <returns></returns>
        private List<Dictionary<string, string>> getDataFromKey(object[] attrs, Keyword key) {
            var rtd = new List<Dictionary<string, string>>();

            int j = 65535;
            for (int i = 0; i < attrs.Length; i++) {
                if (attrs[i] is Keyword && (Keyword)attrs[i] == key) {
                    j = i + 1;
                    break;
                }
            }
            var dic = new Dictionary<string, string>();
            for (; j < attrs.Length; j++) {
                var attr = attrs[j];
                if (attr.GetType() == typeof(string)) {
                    this.addAttr(dic, attr.ToString());
                } else if (attr is Keyword.ENDITEM) {
                    if (dic.Count > 0) rtd.Add(dic);
                    dic = new Dictionary<string, string>();
                } else if (attr is Keyword.LAST) {
                    if (dic.Count > 0) rtd.Add(dic);
                    break;
                }
            }
            return rtd;
        }


        

        /// <summary>
        /// 处理字符串到属性字典中
        /// </summary>
        /// <param name="t">属性字典</param>
        /// <param name="attr">待处理字符串key=value 格式</param>
        private void addAttr(Dictionary<string, string> t, string attr) {
            var attrSplit = attr.Split('=', 2);
            if (attrSplit.Length == 2)
                t.Add(attrSplit[0].ToLower(), attrSplit[1]);
        }

    
    }
}
