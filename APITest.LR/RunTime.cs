using APITest.LR.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using APITest.Core;
using APITest.LR.Handler;

namespace APITest.LR
{
    public class RunTime : iLRParamsAssembleFunctions,iLRFunctions {

        public iLRParamsAssembleFunctions innerFuns;

        public Keyword EXTRARES { get; private set; }
        public Keyword ENDITEM { get; private set; }
        public Keyword LAST { get; private set; }
        public Keyword ITEMDATA { get; private set; }

        public Dictionary<string, string> Parameters;


        public iRunLog Log;
        public RunTime(iRunLog log, Dictionary<string, string> parameters) {
            Log = log;
            Parameters = parameters;
            InitKeyword();
            iLRParamsAssembleFunctions handler = new FunParamsAssembleHandler(log, Parameters);
            innerFuns = new ParamsTransformHandler(handler, log, Parameters);
        }

        private void InitKeyword() {
            EXTRARES = Keyword.EXTRARES;
            ENDITEM = Keyword.ENDITEM;
            LAST = Keyword.LAST;
            ITEMDATA = Keyword.ITEMDATA;
        }

        public void web_add_auto_header(string name, string value) {
            innerFuns.web_add_auto_header(name, value);
        }

        public void web_add_header(string name, string value) {
            innerFuns.web_add_header(name, value);
        }

        public void web_cleanup_auto_headers() {
            innerFuns.web_cleanup_auto_headers();
        }

        public void web_reg_find(params object[] attrs) {
            innerFuns.web_reg_find(attrs);
        }

        public void web_reg_save_param(string paramName, params object[] attrs) {
            innerFuns.web_reg_save_param(paramName, attrs);
        }

        public void web_remove_auto_header(string name, string ImplicitGen = null) {
            innerFuns.web_remove_auto_header(name, ImplicitGen);
        }

        public void web_submit_data(string name, string action, params object[] attrs) {
            innerFuns.web_submit_data(name, action, attrs);
        }

        public void web_custom_request(string name, params object[] attrs) {
            innerFuns.web_custom_request(name, attrs);
        }

        public void web_url(string name, string url, params object[] attrs) {
            innerFuns.web_url(name, url, attrs);
        }

        public void lr_save_string(string value, string name) {
            Parameters[name] = value;
        }

        public string lr_eval_string(string value) {
            return innerFuns.lr_eval_string(value);
        }

        public void lr_output_message(string text, params object[] attrs) {
            Log.Warring("lr_output_message is not support, please use [Log(string msg)]");
            Log.Log(text);
        }

        
    }
}
