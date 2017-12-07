onmessage = function (e) {

    var sharedBuffer = e.data.sharedBuffer;
    const sharedArray = new Int32Array(sharedBuffer);


    LRScriptRun(e.data.script, function (name, args) {

        var lrFun = { name: name, args: args };

        postMessage(lrFun);

        while (sharedArray[0]<=0) {
            Sleep(500);
        }
        sharedArray[0] = 0;
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
            callback(/function\s+(\w+)/.exec(arguments.callee)[1], Array.prototype.slice.apply(arguments));
        }

      




        try {
            eval(code);
        } catch (err) {
            console.log(err);
        }
        postMessage(false);
    }

}

