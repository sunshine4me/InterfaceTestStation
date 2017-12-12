using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace APITest.Web.HttpAgent
{
    public static class FormatFactory {
        public static MemoryStream getProtobufStream(object obj) {
          

            MemoryStream objMS = new MemoryStream();
            Serializer.Serialize(objMS, obj);


            
            var typeInfo = typeof(MessageType).GetField(obj.GetType().Name);
            if(typeInfo == null) {
                Console.WriteLine("对象不能被序列化");
                return null;
            }
            var wsm = new WSMessage();

            
            wsm.type = (MessageType)typeInfo.GetValue(null);
            wsm.message = objMS.ToArray();

            MemoryStream WSMessageMS = new MemoryStream();
            Serializer.Serialize(WSMessageMS, wsm);

            return WSMessageMS;
        }

    }

    [ProtoContract]
    public class WSMessage {
        [ProtoMember(1)]
        public MessageType type { get; set; }
        [ProtoMember(2)]
        public byte[] message { get; set; }

    }
    [ProtoContract]
    public enum MessageType{
        [ProtoEnum(Name = @"Log", Value = 1)]
        Log =1 ,
        [ProtoEnum(Name = @"LRFunction", Value = 2)]
        LRFunction = 2,
        [ProtoEnum(Name = @"LRFunctionRes", Value = 3)]
        LRFunctionRes = 3,
        [ProtoEnum(Name = @"NetWorkMeaasge", Value = 4)]
        NetWorkMeaasge = 4
        
    }


    [ProtoContract]
    public enum LogType {
        [ProtoEnum(Name = @"Log", Value = 1)]
        Log =1,
        [ProtoEnum(Name = @"Warring", Value = 2)]
        Warring =2 ,
        [ProtoEnum(Name = @"Error", Value = 3)]
        Error =3,

        [ProtoEnum(Name = @"Success", Value = 4)]
        Success = 4,
    }

    [ProtoContract]
    public class Log {
        [ProtoMember(1)]
        public LogType type { get; set; }

        [ProtoMember(2)]
        public string logMessage { get; set; }
    }


    [ProtoContract]
    public class NetWorkMeaasge {
        [ProtoMember(1)]
        public int result { get; set; }

        [ProtoMember(2)]
        public string host { get; set; }

        [ProtoMember(3)]
        public string page { get; set; }

        [ProtoMember(4)]
        public string url { get; set; }


        [ProtoMember(5)]
        public string request { get; set; }

        [ProtoMember(6)]
        public string response { get; set; }
        
    }


    [ProtoContract]
    public class LRFunction {
        [ProtoMember(1)]
        public string name { get; set; }

        [ProtoMember(2)]
        public List<string> args { get; set; }
    }

    [ProtoContract]
    public class LRFunctionRes {
        [ProtoMember(1)]
        public string name { get; set; }

        [ProtoMember(2)]
        public bool isContinue { get; set; }

        [ProtoMember(3)]
        public string rtvalue { get; set; }
    }

    
}
