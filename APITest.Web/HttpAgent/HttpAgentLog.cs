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
using System.Text;

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

        public void Network(HttpResponseMessage res,string cookieHeader) {
            var nwm = new NetWorkMeaasge();
            nwm.host =  res.RequestMessage.RequestUri.Host;

            if(res.RequestMessage.RequestUri.Segments.Length>1)
                nwm.page = res.RequestMessage.RequestUri.Segments.Last().Trim('/');
            else
                nwm.page = res.RequestMessage.RequestUri.Host;
            nwm.result = (int)res.StatusCode;
            nwm.url = res.RequestMessage.RequestUri.AbsolutePath;

            StringBuilder request_sb = new StringBuilder();
            request_sb.Append($"{res.RequestMessage.Method.Method} {res.RequestMessage.RequestUri.AbsoluteUri} HTTP/{res.RequestMessage.Version} \r\n");
            request_sb.Append(res.RequestMessage?.Headers?.ToString());
            if (!string.IsNullOrEmpty(cookieHeader)) {
                request_sb.Append($"Cookie: {cookieHeader} \r\n");
            }
           
            request_sb.Append(res.RequestMessage?.Content?.Headers?.ToString());
            request_sb.Append("\r\n");
            request_sb.Append(res.RequestMessage?.Content?.ReadAsStringAsync().Result);
            
            nwm.request = request_sb.ToString();

            StringBuilder response_sb = new StringBuilder();
            response_sb.Append($"HTTP/{res.RequestMessage.Version} {(int)res.StatusCode} \r\n");
            response_sb.Append(res.Headers?.ToString());
            response_sb.Append(res.Content?.Headers?.ToString());
            response_sb.Append("\r\n");
            response_sb.Append(res.Content?.ReadAsStringAsync().Result);

            nwm.response = response_sb.ToString();


            var sm = FormatFactory.getProtobufStream(nwm);
            ws.SendAsync(sm.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public void Success(string msg) {
            var log = new Log() { type = LogType.Success, logMessage = msg };
            var sm = FormatFactory.getProtobufStream(log);
            ws.SendAsync(sm.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public void Warring(string msg) {
            var log = new Log() { type = LogType.Warring, logMessage = msg };


            var sm = FormatFactory.getProtobufStream(log);
            ws.SendAsync(sm.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
