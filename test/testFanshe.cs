using APITest.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using APITest.LR;
using System.Reflection;

namespace test
{
    class myLog : iRunLog {
        public void Error(string msg) {
            
        }

        public void Log(string msg) {
        }

        public void Network(HttpResponseMessage res) {
        }

        public void Warring(string msg) {
        }
    }
    public class testFanshe
    {
        public static void test( string methodName,object[] parameters) {

            

            var log = new myLog();
            var rt = new RunTime(log, new Dictionary<string, string>());

    

            Type t = typeof(APITest.LR.Interface.iLRFunctions);
            var method = t.GetMethod(methodName);
            if (method != null) {
                Type oType = rt.GetType();
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
                            return;
                        }

                    }

                   
                    if (ps[i].ParameterType == typeof(string)) {
                        newParamters[i] = parameters[i];
                    } else {
                        //如果参数不是 string 类型 那么肯定是 object[] 可选参数,讲剩下的参数组装成object[] 参数
                        var objs = new Object[parameters.Length - i];
                        for (int y=0; y < objs.Length; y++) {
                            objs[y] = parameters[y+i];
                        }
                        newParamters[i] = objs;
                        break;
                    }
                    
                }



                oMethod.Invoke(rt, newParamters);

            }

           

            
           

        }
       
    }
}
