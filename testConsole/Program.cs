using System;
using LRengine;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Net;
using Microsoft.CodeAnalysis.CSharp.Scripting;

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
            //                ""URL=https://testerhome.com/account/sign_in"",
            //                ""Resource=0"",
            //                ""RecContentType=text/html"",
            //                ""Referer=https://testerhome.com/"",
            //                ""Snapshot=t3.inf"",
            //                ""Mode=HTML"",
            //                EXTRARES,
            //                ""Url=https://www.google-analytics.com/collect?v=1&_v=j60&a=450370903&t=pageview&_s=2&dl=https%3A%2F%2Ftesterhome.com%2F&ul=zh-cn&de=utf-8&dt=%E7%99%BB%E5%BD%95%20%C2%B7%20TesterHome&sd=24-bit&sr=1280x800&vp=792x544&je=1&fl=20.0%20r0&_u=KGBAgEQ~&jid=&gjid=&cid=364379208.1504577662&tid=UA-45014075-1&_gid=1432109762.1504577662&z=667031373"", ENDITEM,
            //                LAST);
            // return 1;
            //                web_url(""sign_in"",
            //                ""URL=https://testerhome.com/account/sign_in"",
            //                ""Resource=0"",
            //                ""RecContentType=text/html"",
            //                ""Referer=https://testerhome.com/"",
            //                ""Snapshot=t3.inf"",
            //                ""Mode=HTML"",
            //                EXTRARES,
            //                ""Url=https://www.google-analytics.com/collect?v=1&_v=j60&a=450370903&t=pageview&_s=2&dl=https%3A%2F%2Ftesterhome.com%2F&ul=zh-cn&de=utf-8&dt=%E7%99%BB%E5%BD%95%20%C2%B7%20TesterHome&sd=24-bit&sr=1280x800&vp=792x544&je=1&fl=20.0%20r0&_u=KGBAgEQ~&jid=&gjid=&cid=364379208.1504577662&tid=UA-45014075-1&_gid=1432109762.1504577662&z=667031373"", ENDITEM,
            //                LAST);
            //                return 1;
            //}";

            string mycode = @"action(){
web_url(""sign_in"",
                ""URL=Http://testerhome.com/account/sign_in"",
                ""Resource=0"",
                ""RecContentType=text/html"",
                ""Referer=https://testerhome.com/"",
                ""Snapshot=t3.inf"",
                ""Mode=HTML"",
                EXTRARES,
                ""Url=https://www.google-analytics.com/collect?v=1&_v=j60&a=450370903&t=pageview&_s=2&dl=https%3A%2F%2Ftesterhome.com%2F&ul=zh-cn&de=utf-8&dt=%E7%99%BB%E5%BD%95%20%C2%B7%20TesterHome&sd=24-bit&sr=1280x800&vp=792x544&je=1&fl=20.0%20r0&_u=KGBAgEQ~&jid=&gjid=&cid=364379208.1504577662&tid=UA-45014075-1&_gid=1432109762.1504577662&z=667031373"", ENDITEM,
                LAST);
 return 1;
              
}";


            var driver = new runDriver(mycode);
            var rt = driver.runCode();
            Console.WriteLine("result : " + rt);

            Console.ReadLine();
        }
    }
}
