using System;
using System.Collections.Generic;
using System.Text;

namespace LRengine
{
    public class codeIssue
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 严重性
        /// </summary>
        public string Severity { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public int Character { get; set; }
    }
}
