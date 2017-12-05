using APITest.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.WebSockets;
using System.IO;
using ProtoBuf;
using System.Threading;

namespace APITest.Web.HttpAgent
{
    public class HttpAgentLog : iRunLog {

        WebSocket ws;
        public HttpAgentLog(WebSocket ws) {
            this.ws = ws;
        }
        public void Error(string msg) {


            var log = new Log() { type = LogType.Error, logMessage = msg };


            var sm = FormatFactory.getProtobufStream(log);
            ws.SendAsync(sm.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None);

            
        }

        public void Log(string msg) {
            var log = new Log() { type = LogType.Log, logMessage = msg };


            var sm = FormatFactory.getProtobufStream(log);
            ws.SendAsync(sm.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public void Network(HttpResponseMessage res) {
            Log("network....");
        }

        public void Warring(string msg) {
            var log = new Log() { type = LogType.Warring, logMessage = msg };


            var sm = FormatFactory.getProtobufStream(log);
            ws.SendAsync(sm.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
