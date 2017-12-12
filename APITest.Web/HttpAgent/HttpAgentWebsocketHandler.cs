
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
                            res.rtvalue = callLRRuntime(RT, fun);
                            res.isContinue = true;
                            
                            var resMS = FormatFactory.getProtobufStream(res);
                            await webSocket.SendAsync(resMS.ToArray(), WebSocketMessageType.Binary, true, hc.RequestAborted);

                        }
                    } catch (Exception e) {
                        if(e.InnerException==null)
                            log.Error(e.Message);
                        else
                            log.Error(e.InnerException.Message);


                        break;
                    }
                }
                Console.WriteLine("websocket is close!");
                //到这已经断开链接了,再调用 CloseAsync 将报错
                //await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", hc.RequestAborted);

            }

        }


        private static string callLRRuntime(RunTime runtime,LRFunction lrf) {
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


                var methodParamters = oMethod.GetParameters();
                var useParamters = new object[methodParamters.Length];



                //组装参数
                for (int i = 0; i < useParamters.Length; i++) {

                    //传入参数 < 需要的参数 使用默认值
                    if (parameters.Length <=i) {

                        if(methodParamters[i].ParameterType == typeof(string)) {
                            if (methodParamters[i].HasDefaultValue) {
                                useParamters[i] = methodParamters[i].DefaultValue;
                                continue;
                            } else {
                                throw new Exception($"{methodName} 缺少必要参数");
                            }
                        } else {
                            useParamters[i] = new object[0];
                        }
                    }


                    if (methodParamters[i].ParameterType == typeof(string)) {

                        useParamters[i] = parameters[i];
                    } else {
                        //如果参数不是 string 类型 那么肯定是 object[] 可选参数,讲剩下的参数组装成object[] 参数
                        var objs = new Object[parameters.Length - i];
                        for (int y = 0; y < objs.Length; y++) {
                            objs[y] = parameters[y + i];
                        }
                        useParamters[i] = objs;
                        break;
                    }

                }

                var rtn = oMethod.Invoke(runtime, useParamters);
                if (rtn == null)
                    return "True";
                return rtn.ToString();
            } else {
                throw new Exception($"找不到方法 {methodName}");
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
