onmessage = function (e) {

    var endBuffer = e.data.endBuffer;
    const endArray = new Int32Array(endBuffer);

    var sharedBuffer = e.data.sharedBuffer;
    const sharedArray = new Uint16Array(sharedBuffer);


    LRScriptRun(e.data.script, function (name, args) {
        var array2char = function(u16a) {
            var out = "";
            var single;
            for (var i = 0; i < u16a.length; i++) {
                if (u16a[i] == 0) break;
                single = u16a[i].toString(16)
                while (single.length < 4) single = "0".concat(single);
                out += "\\u" + single;
            }
            return eval("'" + out + "'");
        };

        var lrFun = { name: name, args: args };

        postMessage(lrFun);

        while (endArray[0]==0) {
            Sleep(500);
        }
        endArray[0] = 0;

        return array2char(sharedArray);
        
    })

    function Sleep(d) {
        for (var t = Date.now(); Date.now() - t <= d;);
    }


  


    function LRScriptRun(code, callback) {
        var EXTRARES = "@EXTRARES";
        var LAST = "@LAST";
        var ENDITEM = "@ENDITEM";
        var ITEMDATA = "@ITEMDATA";


        function web_url() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function lr_save_string() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function lr_output_message() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }
        function web_reg_find() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function web_reg_save_param() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function web_submit_data() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function web_custom_request() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function web_add_header() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function web_add_auto_header() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function web_remove_auto_header() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function web_cleanup_auto_headers() {
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

        function lr_eval_string() {
            return callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

      


        var rts = { end: true, type: 0, message: "Ending action." };
        try {
            eval(code);
        } catch (err) {
            rts.type = 1;
            rts.message = err + "";
            console.log(err);
        }
        postMessage(rts);
    }

}

