function completionItem() {
    //函数提示
    return [
        {
            label: 'lr_save_string',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "保存字符串到参数（value ,name）",
            insertText: 'lr_save_string("your value", "paramName");'
        },
        {
            label: 'lr_output_message',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "输出日志",
            insertText: 'lr_output_message("your message");'
        },
        {
            label: 'lr_eval_string',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "返回脚本中的参数值",
            insertText: 'lr_eval_string("{paramName}");'
        },
        
        {
            label: 'web_url',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "请求网页",
            insertText: [
                'web_url("newStep",',
                '\t"URL=网页路径",',
                '\t"Referer=",',
                '\t"Mode=URL",',
                '\tLAST);'
            ].join("\r\n")
        },
        {
            label: 'web_submit_data',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "表单提交(POST)",
            insertText: [
                'web_submit_data("newStep",',
                '\t"Action=提交路径",',
                '\t"Referer=",',
                '\t"Mode=URL",',
                '\tITEMDATA,',
                '\t"Name=param1", "Value=param1Value", ENDITEM,',
                '\t"Name=param2", "Value=param2Value", ENDITEM,',
                '\tLAST);'
            ].join("\r\n")
        },
        {
            label: 'web_custom_request',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "创建自定义HTTP请求",
            insertText: [
                'web_custom_request("newStep",',
                '\t"URL=资源路径",',
                '\t"EncType=application/x-www-form-urlencoded",',
                '\t"Referer=",',
                '\t"Body=customRequestBody",',
                '\tLAST);'
            ].join("\r\n")
        },
        {
            label: 'web_add_header',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "向下一个 HTTP 请求中添加自定义信息头",
            insertText: 'web_add_header("headerName", "headerValue");'
        },
        {
            label: 'web_add_auto_header',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "向所有后面的 HTTP 请求中添加自定义信息头",
            insertText: 'web_add_auto_header("headerName", "headerValue");'
        },
        {
            label: 'web_remove_auto_header',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "停止向后面的 HTTP 请求中添加特定的信息头",
            insertText: 'web_remove_auto_header("headerName");'
        },
        {
            label: 'web_cleanup_auto_headers',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "停止向后面的 HTTP 请求中添加自定义信息头",
            insertText: 'web_cleanup_auto_headers();'
        },
        {
            label: 'web_reg_find',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "检查文本",
            insertText: 'web_reg_find("Fail=NotFound", "Search=All", "Text=检查文本");'
        },
        {
            label: 'web_reg_save_param',
            kind: monaco.languages.CompletionItemKind.Function,
            documentation: "从response中保存参数",
            insertText: [
                'web_reg_save_param("paramName",',
                '\t"LB=Left boundary",',
                '\t"RB=Right boundary",',
                '\t"Search=All",',
                '\tLAST);'
            ].join("\r\n")
        },
    ];
}