using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LRengine.report
{
    public class ResourceReport
    {
        public string Url { get; set; }

        public HttpStatusCode StatusCode;

        public bool isFormCache { get; set; }
    }
}
