using APITest.Core;
using APITest.LR.Interface;
using APITest.LR.WebRegEvent;
using System;
using System.Collections.Generic;
using System.Text;

namespace APITest.LR.Handler
{
    internal class FunParamsAssembleHandler : iLRParamsAssembleFunctions {

        private FunctionsCore runTime;
        private iRunLog Log;
        private Dictionary<string, string> Parameters;

        public FunParamsAssembleHandler(iRunLog log, Dictionary<string, string> parameters) {
            Log = log;
            Parameters = parameters;
            runTime = new FunctionsCore(log);
        }

        public void web_add_auto_header(string name, string value) {
            runTime.web_add_auto_header(name, value);
        }

        public void web_add_header(string name, string value) {
            runTime.web_add_header(name, value);
        }

        public void web_cleanup_auto_headers() {
            runTime.web_cleanup_auto_headers();
        }

        public void web_reg_find(params object[] attrs) {
            Dictionary<string, string> attributes = getAttributes(attrs);
            WebRegFindArgs args = new WebRegFindArgs();
            args.Log = this.Log;
            if (attributes.ContainsKey("search")) {
                string search = attributes["search"].ToLower();
                if (search == "body") {
                    args.Search = SearchIn.Body;
                } else if (search == "headers") {
                    args.Search = SearchIn.Headers;
                }
            }

            if (attributes.ContainsKey("fail")) {
                string search = attributes["fail"].ToLower();
                if (search == "found") {
                    args.Fail = FailIf.Found;
                }
            }

            if (attributes.ContainsKey("text")) {
                args.Text = attributes["text"];
            } else {
                Log.Warring("web_reg_find Missing parameter [Text] web_reg_find not register");
                return;
            }

            runTime.web_reg_find(args);
            Log.Log("Registering web_reg_find was successful");
        }

        public void web_reg_save_param(string paramName, params object[] attrs) {
            Dictionary<string, string> attributes = getAttributes(attrs);
            var args = new WebRegSaveParamArgs();
            args.Parameters = this.Parameters;
            args.Log = this.Log;
            args.Name = paramName;

            if (attributes.ContainsKey("search")) {
                string search = attributes["search"].ToLower();
                if (search == "body") {
                    args.Search = SearchIn.Body;
                } else if (search == "headers") {
                    args.Search = SearchIn.Headers;
                }
            }

            if (attributes.ContainsKey("notfound")) {
                string s = attributes["notfound"].ToLower();
                if (s == "warning") {
                    args.NotFound = NotFoundIs.WARNING;
                }
            }

            if (attributes.ContainsKey("saveoffset")) {
                int sf;
                int.TryParse(attributes["saveoffset"], out sf);
                args.SaveOffset = sf;
            }

            if (attributes.ContainsKey("lb")) {
                args.LB = attributes["lb"];
            } else {
                Log.Warring("web_reg_save_param Missing parameter [LB] web_reg_find not register");
                return;
            }

            if (attributes.ContainsKey("rb")) {
                args.RB = attributes["rb"];
            } else {
                Log.Warring("web_reg_save_param Missing parameter [RB] web_reg_find not register");
                return;
            }

            runTime.web_reg_save_param(args);
            Log.Log($"Registering web_reg_save_param(\"{args.Name}\") was successful");
        }

        public void web_remove_auto_header(string name, string ImplicitGen = null) {
            runTime.web_remove_auto_header(name);
        }

        public void web_submit_data(string name, string action, params object[] attrs) {

            Log.Log($"web_submit_data({name}) started");
            var targetUri = new Uri(action.Split('=', 2)[1]);


            //处理Referer;
            Dictionary<string, string> attributes = getAttributes(attrs);
            if (attributes.ContainsKey("referer")) {
                web_add_header("Referer", attributes["referer"]);
            }

            var datas = getDataFromKey(attrs, Keyword.ITEMDATA);
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            foreach (var d in datas) {
                string k = null;
                string v = null;
                if (d.TryGetValue("name", out k) && d.TryGetValue("value", out v))
                    paramList.Add(new KeyValuePair<string, string>(k, v));
            }

            runTime.web_submit_data(name, targetUri, paramList);
        }


        public void web_custom_request(string name, params object[] attrs) {
          
            
            Dictionary<string, string> attributes = getAttributes(attrs);

            var url = attributes.GetValueOrDefault("url");
            if (string.IsNullOrEmpty(url)) {
                Log.Error("The \"URL\" argument is missing or empty");
            }

            if (attributes.ContainsKey("referer")) {
                web_add_header("Referer", attributes["referer"]);
            }

            Log.Log($"web_custom_request({name}) started");

            var targetUri = new Uri(url);

            var body = attributes.GetValueOrDefault("body");

            runTime.web_custom_request(name, targetUri, body);
        }

        public void web_url(string name, string url, params object[] attrs) {
            Log.Log($"web_url({name}) started");
            var targetUri = new Uri(url.Split('=', 2)[1]);


            //处理Referer;
            Dictionary<string, string> attributes = getAttributes(attrs);
            if (attributes.ContainsKey("referer")) {
                web_add_header("Referer", attributes["referer"]);
            }

            runTime.web_url(name, targetUri);
        }


        //额外参数相关
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
        /// 获取追加数据(key 之后)
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

        public string lr_eval_string(string value) {
            return value;
        }

        
    }
}
