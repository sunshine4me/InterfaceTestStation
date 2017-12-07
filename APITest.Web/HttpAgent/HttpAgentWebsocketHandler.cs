
using APITest.LR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APITest.Web.HttpAgent {
    public class HttpAgentWebsocketHandler
    {
        static async Task HttpTest(HttpContext hc, Func<Task> n) {
            if (!hc.WebSockets.IsWebSocketRequest) return;
            using (var webSocket = await hc.WebSockets.AcceptWebSocketAsync()) {

                var log = new HttpAgentLog(webSocket);
                RunTime RT = new RunTime(log, new Dictionary<string, string>());
                
                byte[] ReceiveBuffer = new byte[4096];
                var ReceiveSegment = new ArraySegment<byte>(ReceiveBuffer);
                while (webSocket.State == WebSocketState.Open) {
                    try {
                        var msg = await ReceiveStreamAsync(webSocket, ReceiveSegment, hc.RequestAborted);
                        if(msg.type == MessageType.LRFunction) {
                            var fun = ProtoBuf.Serializer.Deserialize<LRFunction>(new MemoryStream(msg.message));

                            LRFunctionRes res = new LRFunctionRes() { name = fun.name};
                            if (callLRRuntime(RT, fun)) {
                                res.isContinue = true;
                            } else {
                                res.isContinue = false;
                            }
                            var resMS = FormatFactory.getProtobufStream(res);
                            await webSocket.SendAsync(resMS.ToArray(), WebSocketMessageType.Binary, true, hc.RequestAborted);

                        }
                    } catch (Exception e) {
                        log.Error(e.InnerException.Message);


                        break;
                    }
                }
                Console.WriteLine("websocket is close!");
                //到这已经断开链接了,再调用 CloseAsync 将报错
                //await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", hc.RequestAborted);

            }

        }


        private static bool callLRRuntime(RunTime runtime,LRFunction lrf) {
            string methodName = lrf.name;
            List<string> args = lrf.args;
            var parameters = new object[args.Count];
            for(int i=0;i< parameters.Length; i++) {
                switch (args[i]) {
                    case "@EXTRARES":
                        parameters[i] = Keyword.EXTRARES;
                        break;
                    case "@LAST":
                        parameters[i] = Keyword.LAST;
                        break;
                    case "@ENDITEM":
                        parameters[i] = Keyword.ENDITEM;
                        break;
                    case "@ITEMDATA":
                        parameters[i] = Keyword.ITEMDATA;
                        break;
                    default:
                        parameters[i] = args[i];
                        break;
                }
            }



            Type t = typeof(APITest.LR.Interface.iLRFunctions);
            var method = t.GetMethod(methodName);
            if (method != null) {
                Type oType = runtime.GetType();
                var oMethod = oType.GetMethod(method.Name);


                var ps = oMethod.GetParameters();
                var newParamters = new object[ps.Length];



                //组装参数
                for (int i = 0; i < newParamters.Length; i++) {

                    //需要参数  > 传入参数 使用默认值
                    if (i >= parameters.Length) {

                        if (ps[i].HasDefaultValue) {
                            newParamters[i] = ps[i].DefaultValue;
                            continue;
                        } else {
                            Console.WriteLine("参数不够");
                            return false;
                        }

                    }


                    if (ps[i].ParameterType == typeof(string)) {
                        newParamters[i] = parameters[i];
                    } else {
                        //如果参数不是 string 类型 那么肯定是 object[] 可选参数,讲剩下的参数组装成object[] 参数
                        var objs = new Object[parameters.Length - i];
                        for (int y = 0; y < objs.Length; y++) {
                            objs[y] = parameters[y + i];
                        }
                        newParamters[i] = objs;
                        break;
                    }

                }



                oMethod.Invoke(runtime, newParamters);
                return true;
            } else {
                Console.WriteLine("没有方法:" + methodName);
                return false;
            }
                
        }



        private static Task SendStringAsync(System.Net.WebSockets.WebSocket socket, string data, CancellationToken ct = default(CancellationToken)) {
            
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static Task SendLogAsync(System.Net.WebSockets.WebSocket socket,  CancellationToken ct = default(CancellationToken)) {

            var log = new Log() { type  = LogType.Error,logMessage ="我i操嗷嗷嗷啊" };

            MemoryStream messageMS = new MemoryStream();
            Serializer.Serialize(messageMS, log);


            var wsm = new WSMessage();
            wsm.type = MessageType.Log;
            wsm.message = messageMS.ToArray();

            MemoryStream WSMessageMS = new MemoryStream();
            Serializer.Serialize(WSMessageMS, wsm);

            return socket.SendAsync(WSMessageMS.ToArray(), WebSocketMessageType.Binary, true, ct);
        }



        private static async Task<string> ReceiveStringAsync(System.Net.WebSockets.WebSocket socket, ArraySegment<byte> buffer, CancellationToken ct = default(CancellationToken)) {
            using (var ms = new MemoryStream()) {
                WebSocketReceiveResult result;
                do {
                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text) {
                    return null;
                }

                using (var reader = new StreamReader(ms, Encoding.UTF8)) {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private static async Task<WSMessage> ReceiveStreamAsync(System.Net.WebSockets.WebSocket socket, ArraySegment<byte> buffer, CancellationToken ct = default(CancellationToken)) {
            using (var ms = new MemoryStream()) {


                WebSocketReceiveResult result;
                do {
                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                var msg = ProtoBuf.Serializer.Deserialize<WSMessage>(ms);
                return msg;
            }

        }


        public static void MapControl(IApplicationBuilder app) {
            app.Use(HttpAgentWebsocketHandler.HttpTest);
        }
    }
}
