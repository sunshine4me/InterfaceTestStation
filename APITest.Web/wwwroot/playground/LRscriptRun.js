onmessage = function (e) {

   

    LRScriptRun(e.data.script, function (name, args) {

        var websocket = e.data.websocket;
        var LRFunction = e.data.LRFunction;
        var WSMessage = e.data.WSMessage;

        var lrFun = LRFunction.create({ name: name, args: args });
        var lrBuffer = LRFunction.encode(lrFun).finish();

        var msg = WSMessage.create({ type: MessageType.values.LRFunction, message: lrBuffer });

        var msgBuff = WSMessage.encode(msg).finish();
        websocket.resOver = false;
        websocket.send(msgBuff);

        while (!websocket.resOver) {
            Sleep(500);
        }
    })

  


    function LRScriptRun(code, callback) {
        var EXTRARES = "@EXTRARES";
        var LAST = "@LAST";
        var ENDITEM = "@ENDITEM";
        var ITEMDATA = "@ITEMDATA";


        function web_url() {
            callback("web_url", arguments);
        }

        try {
            eval(code);
        } catch (err) {
            console.log(err);
        }

    }

}

