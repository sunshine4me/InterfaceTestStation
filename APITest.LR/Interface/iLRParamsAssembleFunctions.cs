using System;
using System.Collections.Generic;
using System.Text;

namespace APITest.LR.Interface
{
    public interface iLRParamsAssembleFunctions
    {
        void web_reg_find(params object[] attrs);

        void web_reg_save_param(string paramName, params object[] attrs);

        void web_url(string name,string url, params object[] attrs);

        void web_submit_data(string name, string action, params object[] attrs);

        void web_custom_request(string name, params object[] attrs);

        void web_add_header(string name, string value);

        void web_add_auto_header(string name, string value);

        void web_remove_auto_header(string name, string ImplicitGen = null);

        void web_cleanup_auto_headers();

        string lr_eval_string(string value);
    }
}
