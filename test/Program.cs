using APITest.Core;
using APITest.ScriptFactory;
using System;
using System.Net.Http;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


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

web_add_header(""Content-type"",""asdasd"");
web_reg_save_param(""test"",""Search=Body"",""NotFound=ERROR"",""LB=href=\"""",""RB=\"""",""SaveOffset=1"");
web_reg_find(""Fail=NotFound"",""Search=Body"",""Text=testerhome"");
web_submit_data(""login"", 
        ""Action=http://testerhome.com/account/sign_in"", 
        ""Method=POST"", 
        ""RecContentType=text/html"", 
        ""Referer=http://testerhome.com/account/sign_in"", 
        ""Mode=URL"", 
        ITEMDATA, 
        ""Name=utf8"", ""Value=✓"", ENDITEM, 
        ""Name=user[login]"", ""Value=176560744@qq.com"", ENDITEM, 
        ""Name=user[password]"", ""Value=Cpic1234"", ENDITEM, 
        ""Name=user[remember_me]"", ""Value=0"", ENDITEM, 
        ""Name=commit"", ""Value=登录"", ENDITEM,
        LAST);

web_custom_request(""login"", 
        ""URL=http://testerhome.com/account/sign_in"", 
        ""Method=POST"", 
        ""RecContentType=text/html"", 
        ""Referer=http://testerhome.com/account/sign_in"", 
        ""Mode=URL"", 
        ""Body=utf8=%E2%9C%93&user%5Blogin%5D=176560744%40qq.com&user%5Bpassword%5D=Cpic1234&user%5Bremember_me%5D=0&commit=%E7%99%BB%E5%BD%95"",
        LAST);

Log.Log(""nimanaaaaaaaaaaaa"");
 return 1;
              
}";
            ScriptEngine driver = new ScriptEngine(mycode);
            
            var rt = driver.runCode(new LR_log(),new System.Collections.Generic.Dictionary<string, string>());
            Console.WriteLine("result : " + rt);

            Console.ReadLine();
        }
    }

    public class LR_log : iRunLog {
        public void Error(string msg) {
            Console.WriteLine(msg);
        }

        public void Log(string msg) {
            Console.WriteLine(msg);
        }

        public void ReportResponse(HttpResponseMessage res) {
            Console.WriteLine("ReportResponse");
        }

        public void Warring(string msg) {
            Console.WriteLine(msg);
        }
    }
}
