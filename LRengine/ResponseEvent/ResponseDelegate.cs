using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace LRengine.ResponseEvent
{
    public delegate bool ResponseDelegate(ResponseDelegateArgs args);
    public class ResponseDelegateArgs
    {
        public HttpResponseMessage Response { get; set; }
    }

    public class RegisterEvent {
        public ResponseDelegate DelegateEvent { get; set; }

        public ResponseDelegateArgs EventArgs { get; set; }

        public bool Execute() {
            return DelegateEvent(EventArgs);
        }
    }
}
