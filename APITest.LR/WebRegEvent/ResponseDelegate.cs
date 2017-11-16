using APITest.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace APITest.LR.WebRegEvent {
    public delegate bool ResponseDelegate(ResponseDelegateArgs args);
    public class ResponseDelegateArgs
    {
        public HttpResponseMessage Response { get; set; }
        public iRunLog Log { get; set; }
    }

    public class RegisterEvent {
        public ResponseDelegate DelegateEvent { get; set; }

        public ResponseDelegateArgs EventArgs { get; set; }

        public bool Execute() {
            return DelegateEvent(EventArgs);
        }
    }
}
