// Write your JavaScript code.

// Write your Javascript code.
$(function () {
    
    
    $.fn.fillData = function (data) {
        if (this.length == 0) return;

        var jqObj = this.eq(0);
        jqObj[0].reset();
        var fromData;
        if ($.isPlainObject(data))
            fromData = data;
        else
            fromData = eval("(" + data + ")");
        for (var i in fromData) {
            var _input = jqObj.find("[name='" + i + "']");
            if (_input.is("select"))
                jqObj.find("[name='" + i + "']").val(data[i] + "");
            else
                jqObj.find("[name='" + i + "']").val(data[i]);
        }
    };

    $.confirmModel = function (_param) {

        if ($.isPlainObject(_param)) {
            var param = { title: "提示信息", body: "您确认要删除吗？", yes: "确认", no: "取消", submit: function () { } };
            $.extend(param, _param);

            $("#confirmModal .confirm-title").text(param.title);
            $("#confirmModal .confirm-body").text(param.body);
            $("#confirmModal .confirm-yes").text(param.yes);
            $("#confirmModal .confirm-no").text(param.no);

            $("#confirmModal .confirm-yes").unbind("click");

            $("#confirmModal .confirm-yes").click(function () {
                if (param.submit() != false)
                    $("#confirmModal").modal("hide");
            });

            $("#confirmModal").modal("show");
        } else {
            if (_param == "hide") {
                $("#confirmModal").modal("hide");
            }
        }


    };
})

Date.prototype.Format = function (fmt) { //author: meizz
    function pad(num, n) {
        var i = (num + "").length;
        while (i++ < n) num = "0" + num;
        return num;
    }
    var o = {
        "M+": this.getMonth() + 1,                 //月份
        "d+": this.getDate(),                    //日
        "h+": this.getHours(),                   //小时
        "m+": this.getMinutes(),                 //分
        "s+": this.getSeconds(),                 //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        "S": pad(this.getMilliseconds(), 3)            //毫秒
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) { return pair[1]; }
    }
    return (false);
}

