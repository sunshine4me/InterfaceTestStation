using APITest.Core;
using APITest.LR.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace APITest.LR.Handler {


    /// <summary>
    /// 参数控制
    /// </summary>
    internal class ParamsTransformHandler : iLRFunctions {

        
        private Dictionary<string, string> Parameters;
        iLRFunctions innerFuns;
        iRunLog Log;
        public ParamsTransformHandler(iLRFunctions fun, iRunLog log,Dictionary<string, string> parameters) {
            Log = log;
            innerFuns = fun;
            Parameters = parameters;
        }

        #region 不需替换参数的Fun

        public void lr_output_message(string text, params object[] attrs) {
            innerFuns.lr_output_message(text, attrs);
        }


        public void lr_save_string(string value, string name) {
            innerFuns.lr_save_string(value,name);
        }
        #endregion

        public string lr_eval_string(string value) {
            value = InjectionParam(value);
            return innerFuns.lr_eval_string(value);
        }

        public void web_add_auto_header(string name, string value) {
            name = InjectionParam(name);
            value = InjectionParam(value);
            innerFuns.web_add_auto_header(name, value);
        }

        public void web_add_header(string name, string value) {
            name = InjectionParam(name);
            value = InjectionParam(value);
            innerFuns.web_add_header(name, value);
        }

        public void web_cleanup_auto_headers() {
            innerFuns.web_cleanup_auto_headers();
        }

        public void web_custom_request(string name, params object[] attrs) {
            name = InjectionParam(name);
            InjectionParam(attrs);
            innerFuns.web_custom_request(name,attrs);
        }

        public void web_reg_find(params object[] attrs) {
            InjectionParam(attrs);
            innerFuns.web_reg_find(attrs);
        }

        public void web_reg_save_param(string paramName, params object[] attrs) {
            paramName = InjectionParam(paramName);
            InjectionParam(attrs);
            innerFuns.web_reg_save_param(paramName, attrs);
        }

        public void web_remove_auto_header(string name, string ImplicitGen = null) {
            name = InjectionParam(name);
            ImplicitGen = InjectionParam(ImplicitGen);
            innerFuns.web_remove_auto_header(name, ImplicitGen);
        }

        public void web_submit_data(string name, string action, params object[] attrs) {
            name = InjectionParam(name);
            action = InjectionParam(action);
            InjectionParam(attrs);
            innerFuns.web_submit_data(name, action, attrs);
        }

        public void web_url(string name, string url, params object[] attrs) {
            name = InjectionParam(name);
            url = InjectionParam(url);
            InjectionParam(attrs);
            innerFuns.web_url(name, url, attrs);
        }

        private string InjectionParam(string v) {
            if (v == null) return v;
            foreach (var p in Parameters) {
                if(v.IndexOf("{" + p.Key + "}") >= 0) {
                    Log.Log($"Parameter Substitution: parameter \"{p.Key}\" = \"{p.Value}\"");
                    v = v.Replace("{" + p.Key + "}", p.Value);
                }
            }
            return v;
        }

        /// <summary>
        /// 转换{参数}
        /// </summary>
        private void InjectionParam(object[] attrs) {

            for(int i=0;i< attrs.Length; i++) {
                if (attrs[i].GetType() == typeof(string)) {
                    string v = attrs[i] as string;
                    attrs[i] = InjectionParam(v);
                }
            }
           
        }

    }
}
