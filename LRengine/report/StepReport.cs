using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LRengine.report
{
    public class StepReport
    {
        //public StepReport() {
        //    Resources = new List<ResourceReport>();
        //}
        public string Name { get; set; }
        public int HeaderFlow { get; set; }
        public int BodyFlow { get; set; }
        
        public HttpStatusCode StatusCode;

        public ConcurrentDictionary<int,int> TotalStatusCodes;

        public List<ResourceReport> Resources;
    }
}
