using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using LRengine.extend;
using LRengine.httpHandler;

namespace LRengine
{
    public class functionLibrary {

        public iRunLog log;

        public functionLibrary(iRunLog log) {

            HttpMessageHandler handler = new redirectHandler(3,new HttpClientHandler() { AllowAutoRedirect = false});
            handler = new DecompressionHandler(System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate, handler);

            //自动处理gzip
            //HttpClientHandler handler = new HttpClientHandler() {
            //    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            //};
            httpClient = new HttpClient(handler);
            //httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");

           

            EXTRARES = Keyword.EXTRARES;
            ENDITEM = Keyword.ENDITEM;
            LAST = Keyword.LAST;

            this.log = log;
        }


        private HttpClient httpClient;
        public Keyword EXTRARES { get; private set; }
        public Keyword ENDITEM { get; private set; }
        public Keyword LAST { get; private set; }

        

        public void logLine(int line) {
            log.Log($"action({line}):");
        }



        public void web_url(string StepName, string url, params object[] attrs) {
            //return;
            //对loadrunner 参数的预处理
            Dictionary<string, string> attributes = getAttributes(attrs);
            List<Dictionary<string, string>> resourceAttributes = getResourceAttributes(attrs);

            var targetUri = new Uri(url.Split('=', 2)[1]);
            
            if (attributes.ContainsKey("RecContentType")) {
                // TODO: attributes-RecContentType 处理
            }
            
            var hrm = new HttpRequestMessage(HttpMethod.Get, targetUri);

            if (attributes.ContainsKey("Referer")) {
                hrm.Headers.Referrer = new Uri(attributes["Referer"]);
            }

            httpClient.SendAsync(hrm);


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
                if (attr is Keyword.EXTRARES) break;
            }
            return rtd;
        }

        /// <summary>
        /// ResourceAttributes资源处理
        /// </summary>
        /// <param name="attrs"></param>
        /// <returns></returns>
        private List<Dictionary<string, string>> getResourceAttributes(object[] attrs) {
            var rtd = new List<Dictionary<string, string>>();

            int j = 65535;
            for (int i = 0; i < attrs.Length; i++) {
                if (attrs[i] is Keyword.EXTRARES) {
                    j = i + 1;
                    break;
                }
            }

            for (; j < attrs.Length; j++) {
                var attr = attrs[j];
                var dic = new Dictionary<string, string>();
                if (attr.GetType() == typeof(string)) {
                    this.addAttr(dic, attr.ToString());
                } else if (attr is Keyword.ENDITEM) {
                    if (dic.Count > 0) rtd.Add(dic);
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
                t.Add(attrSplit[0], attrSplit[1]);
        }
    }
}
