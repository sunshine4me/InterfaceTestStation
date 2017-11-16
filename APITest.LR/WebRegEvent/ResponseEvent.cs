using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace APITest.LR.WebRegEvent {
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
            
            if (sb.ToString().IndexOf(_args.Text) >= 0) {
                _args.Log.Log($"web_reg_find successful for \"Text={_args.Text}\"");
                return true;
            }
            _args.Log.Error($"\"Text={_args.Text}\" not found for web_reg_find");
            return false;
        }

        public static bool WebRegSaveParam(ResponseDelegateArgs args) {
            var _args = args as WebRegSaveParamArgs;

            StringBuilder sb = new StringBuilder();
            if (_args.Search == (SearchIn.All | SearchIn.Headers)) {
                sb.Append(_args.Response.Headers.ToString());
                sb.Append(_args.Response.Content.Headers.ToString());
            }
            if (_args.Search == (SearchIn.All | SearchIn.Body)) {
                sb.Append(_args.Response.Content.ReadAsStringAsync().Result);
            }

            string pattern = $"(?<={Regex.Escape(_args.LB)}).*?(?={Regex.Escape(_args.RB)})";
            Regex re = new Regex(pattern);
            var mc = re.Match(sb.ToString());
       

            for (int i=0;i<_args.SaveOffset;i++) {
                if (mc.Success)
                    mc = mc.NextMatch();
                else {
                    _args.Log.Error($"SaveOffset({_args.SaveOffset}) exceeds the found value length({i}) for parameter \"{_args.Name}\".");
                    return false;
                }
                    
            }
            if (mc.Success) {
                _args.Parameters[_args.Name] = mc.Value;
                _args.Log.Log($"save parameter \"{_args.Name} = {mc.Value}\"");
                return true;

            } else {
                _args.Log.Error($"Not match found for parameter \"{_args.Name}\"");
                return false;
            }
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

    public class WebRegSaveParamArgs : ResponseDelegateArgs {
        public WebRegSaveParamArgs() {
            Search = SearchIn.All;
            NotFound = NotFoundIs.ERROR;
        }
        public string Name { get; set; }
        public SearchIn Search { get; set; }
        public string LB { get; set; }
        public string RB { get; set; }
        public int SaveOffset { get; set; }
        public NotFoundIs NotFound { get; set; }

        public Dictionary<string, string> Parameters;
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

    public enum NotFoundIs {
        ERROR,
        WARNING
    }
}
