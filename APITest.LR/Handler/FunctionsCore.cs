using APITest.Core;
using APITest.LR.WebRegEvent;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace APITest.LR.Handler {
    internal class FunctionsCore
    {
        private HttpClientCore httpClient;
        protected iRunLog Log;
        private List<RegisterEvent> responseEvents = new List<RegisterEvent>();
        

        private Dictionary<string, string> customAutoHeader = new Dictionary<string, string>();
        private Dictionary<string, string> nextRequestHeader = new Dictionary<string, string>();
        


        public FunctionsCore(iRunLog log) {
            Log = log;
            httpClient = new HttpClientCore(Log);
        }

        public void web_add_header(string name, string value) {
            nextRequestHeader[name] = value;
        }

        public void web_add_auto_header(string name, string value) {
            customAutoHeader[name] = value;
            httpClient.DefaultRequestHeaders.Remove(name);
            httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public void web_remove_auto_header(string name) {
            httpClient.DefaultRequestHeaders.Remove(name);
        }

        public void web_cleanup_auto_headers() {
            foreach (var key in customAutoHeader) {
                httpClient.DefaultRequestHeaders.Remove(key.Key);
            }
        }


        public void web_reg_find(WebRegFindArgs args) {
            var del = new ResponseDelegate(ResponseEventFactory.WebRegFind);
            responseEvents.Add(new RegisterEvent() { DelegateEvent = del, EventArgs = args });
           
        }

        public void web_reg_save_param(WebRegSaveParamArgs args) {
            var del = new ResponseDelegate(ResponseEventFactory.WebRegSaveParam);
            responseEvents.Add(new RegisterEvent() { DelegateEvent = del, EventArgs = args });

            
        }


        public void web_url(string name, Uri targetUri) {

            var res = httpClient.HttpGet(targetUri, nextRequestHeader);

            completeResponseEvent(res);
        }

        public void web_submit_data(string name, Uri targetUri, IEnumerable<KeyValuePair<string, string>> paramList) {
            
            var res = httpClient.HttpPostForm(targetUri, nextRequestHeader, paramList);
            completeResponseEvent(res);
        }


        public void web_custom_request(string name, Uri targetUri, string body) {
            var content = new StringContent(body);
            var res = httpClient.HttpCustom(targetUri,HttpMethod.Post, nextRequestHeader, content);
            completeResponseEvent(res);
        }




        private void completeResponseEvent(HttpResponseMessage response) {

            foreach (var eve in responseEvents) {
                eve.EventArgs.Response = response;
                if (!eve.Execute()) {
                    throw new Exception("case war \"ERROR\", Ending action");
                }
            }
            responseEvents.Clear();
        }


       
    }
}
