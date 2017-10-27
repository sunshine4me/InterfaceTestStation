using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LRengine.Report;

namespace LRengine
{
    public class runDriver
    {
        private string _code;
        public string code {
            get { return _code; }
            set {
                _code = value;
                this.codeIssues = Compile();
            }
        }
        public iRunLog log { get; set; }
        public IEnumerable<CodeIssue> codeIssues;
        private FunctionLibrary funLib;

        public runDriver(string code) :this(code,new autoLog()){
        }
        public runDriver(string code,iRunLog log) {
            this.code = code;
            this.log = log;
            funLib = new FunctionLibrary(this.log);
        }

        public object runCode(string logPath) {

            SyntaxTree tree = CSharpSyntaxTree.ParseText("int " + code);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            //尝试修改代码插入一些函数,暂时不用
            //StringBuilder codeTmp = new StringBuilder();

            //foreach (var node in root.ChildNodes().First().ChildNodes().OfType<BlockSyntax>().First().ChildNodes()) {
            //    var stmt1 = SyntaxFactory.ParseStatement("logLine(1);");
            //    var sss = root.InsertNodesBefore(node,new[] { stmt1 });



            //    int codeLine = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            //    log.Log(root.ToFullString());
            //    codeTmp.Append($"logLine({codeLine});");
            //    codeTmp.Append(node.ToFullString());
            //}
            //log.Log(codeTmp.ToString());
            //var cc = root.ChildNodes().FirstOrDefault().ChildNodes().Skip(2).FirstOrDefault().ToFullString().TrimStart('{').TrimEnd('}');

            //var result = CSharpScript.RunAsync(root.ChildNodes().First().ChildNodes().OfType<BlockSyntax>().First().ToFullString(),
            //               globals: funLib);

            if (codeIssues.Any()) {
                Console.WriteLine("代码存在以下错误,请进行处理:");
                foreach (var codeIssue in codeIssues) {
                    string issue = $"严重性:{codeIssue.Severity}, 错误代码: {codeIssue.Id}, 说明: {codeIssue.Message},位置: {codeIssue.Line} 行 {codeIssue.Character}列";
                    Console.WriteLine(issue);
                }
                return null;
            }

            var result = CSharpScript.RunAsync(root.ChildNodes().First().ChildNodes().OfType<BlockSyntax>().First().ToFullString(),
                           globals: funLib);
            return result.Result.ReturnValue;
        }

        private void RegenerateCode(IEnumerable<SyntaxNode> nodes) {
            
            foreach (var node in nodes) {
                if (node.ChildNodes().Any()) RegenerateCode(node.ChildNodes());
            }
        }

        private IEnumerable<CodeIssue> Compile() {

            // 从源代码生成语法树
            SyntaxTree tree = CSharpSyntaxTree.ParseText("int " + code);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var rt = from t in root.GetDiagnostics()
                     select new CodeIssue {
                         Id = t.Id,
                         Message = t.GetMessage(),
                         Severity = t.Severity.ToString(),
                         Line = t.Location.GetLineSpan().StartLinePosition.Line,
                         Character = t.Location.GetLineSpan().StartLinePosition.Character,
                     };

            return rt.ToList();
        }
    }

    public class autoLog : iRunLog {
        public void Error(string msg) {
            Console.WriteLine("Error : " + msg);
        }

        public void Log(string msg) {
            Console.WriteLine(msg);
        }

        public void StepLog(StepReport report) {
            Console.WriteLine(report.Name+" run over.");
        }

        public void Warring(string msg) {
            Console.WriteLine("Warring : " + msg);
        }
    }
}
