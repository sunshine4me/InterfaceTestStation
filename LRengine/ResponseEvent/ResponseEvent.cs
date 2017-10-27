using System;
using System.Collections.Generic;
using System.Text;

namespace LRengine.ResponseEvent
{
    public class ResponseEventFactory
    {
        public static bool WebRegFind(ResponseDelegateArgs args) {
            var _args = args as WebRegFindArgs;

            StringBuilder sb = new StringBuilder();
            if(_args.Search== (SearchIn.All | SearchIn.Headers)) {
                sb.Append(_args.Response.Headers.ToString());
                sb.Append(_args.Response.Content.Headers.ToString());
            }
            if (_args.Search == (SearchIn.All | SearchIn.Body)) {
                sb.Append(_args.Response.Content.ReadAsStringAsync().Result);
            }
            
            if (sb.ToString().IndexOf(_args.Text) >= 0)
                return true;
            return false;
        }
    }

 

    public class WebRegFindArgs : ResponseDelegateArgs {
        public WebRegFindArgs() {
            Search = SearchIn.All;
            Fail = FailIf.NotFound;
            Text = "";
        }
        public SearchIn Search { get; set; }
        public string Text { get; set; }
        public FailIf Fail { get; set; }
    }

    public enum SearchIn {
        All,
        Body,
        Headers
    }

    public enum FailIf {
        Found,
        NotFound
    }
}
