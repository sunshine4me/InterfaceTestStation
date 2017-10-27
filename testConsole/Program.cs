using System;
using LRengine;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Net;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Text;
using LRengine.Report;
using System.Collections.Generic;

namespace testConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            //var s0 = CSharpScript.Create("int x = 1;");

       

            //s0.RunAsync();
            //var s1 = s0.ContinueWith("int y = 2;");
            //s0.RunAsync();
            //var s2 = s1.ContinueWith<int>("x + y");
            //Console.WriteLine(s2.RunAsync().Result.ReturnValue);


            //WebClient client = new WebClient();

    

            //return;

            Console.WriteLine("Hello World!");
          

//            string mycode = @"action(){
//web_url(""sign_in"",
//                ""URL=Http://testerhome.com/account/sign_in"",
//                ""Resource=0"",
//                ""RecContentType=text/html"",
//                ""Referer=https://testerhome.com/"",
//                ""Snapshot=t3.inf"",
//                ""Mode=HTML"",
//                EXTRARES,
//                ""Url=https://www.google-analytics.com/collect?v=1&_v=j60&a=450370903&t=pageview&_s=2&dl=https%3A%2F%2Ftesterhome.com%2F&ul=zh-cn&de=utf-8&dt=%E7%99%BB%E5%BD%95%20%C2%B7%20TesterHome&sd=24-bit&sr=1280x800&vp=792x544&je=1&fl=20.0%20r0&_u=KGBAgEQ~&jid=&gjid=&cid=364379208.1504577662&tid=UA-45014075-1&_gid=1432109762.1504577662&z=667031373"", ENDITEM,
//                LAST);

//web_url(""sign_in"",
//                ""URL=Http://testerhome.com/account/sign_in"",
//                ""Resource=0"",
//                ""RecContentType=text/html"",
//                ""Referer=https://testerhome.com/"",
//                ""Snapshot=t3.inf"",
//                ""Mode=HTML"",
//                EXTRARES,
//                ""Url=https://www.google-analytics.com/collect?v=1&_v=j60&a=450370903&t=pageview&_s=2&dl=https%3A%2F%2Ftesterhome.com%2F&ul=zh-cn&de=utf-8&dt=%E7%99%BB%E5%BD%95%20%C2%B7%20TesterHome&sd=24-bit&sr=1280x800&vp=792x544&je=1&fl=20.0%20r0&_u=KGBAgEQ~&jid=&gjid=&cid=364379208.1504577662&tid=UA-45014075-1&_gid=1432109762.1504577662&z=667031373"", ENDITEM,
//                LAST);

// return 1;
              
//}";


            string mycode = @"action(){
web_reg_find(""Fail=NotFound"",""Search=Body"",""Text="");

web_submit_data(""login"", 
        ""Action=https://testerhome.com/account/sign_in"", 
        ""Method=POST"", 
        ""RecContentType=text/html"", 
        ""Referer=https://testerhome.com/account/sign_in"", 
        ""Mode=HTML"", 
        ITEMDATA, 
        ""Name=utf8"", ""Value=✓"", ENDITEM, 
        ""Name=user[login]"", ""Value=176560744@qq.com"", ENDITEM, 
        ""Name=user[password]"", ""Value=Cpic1234"", ENDITEM, 
        ""Name=user[remember_me]"", ""Value=0"", ENDITEM, 
        ""Name=commit"", ""Value=登录"", ENDITEM,
        LAST);
 return 1;
              
}";
            myLog log = new myLog();

            var driver = new runDriver(mycode, log);
            var rt = driver.runCode("D:\\testHttpclient");
            Console.WriteLine("result : " + rt);
            
            Console.ReadLine();
        }
    }



    public class myLog : iRunLog {
        public List<StepReport> reports = new List<StepReport>();
        public void Error(string msg) {
            Console.WriteLine("Error : " + msg);
        }

        public void Log(string msg) {
            Console.WriteLine(msg);
        }

        public void StepLog(StepReport report) {
            reports.Add(report);
        }

        public void Warring(string msg) {
            Console.WriteLine("Warring : " + msg);
        }
    }
}
