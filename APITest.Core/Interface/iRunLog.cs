
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace APITest.Core {
    public interface iRunLog
    {
        void Log(string msg);
        void Warring(string msg);
        void Error(string msg);
        void Network(HttpResponseMessage res);
    }

    
}
