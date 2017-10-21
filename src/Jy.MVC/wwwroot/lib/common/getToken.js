function getToken(username, password,memberme) {
    var postData = { "model": { "UserName": username, "Password": password, "RememberMe": memberme } };
    $.ajax({
        type: "Post",
        url: "/Login/GetToken",
        data: postData,
        success: function (data) {
           // debugger
            if (data.result == "Success") {
                //$.cookie("token", data.token,{ path: '/', expires: 30 });
                $.ajaxSetup({
                    headers: {
                        "Authorization": "Bearer " + data.token
                    }
                });
                $.post("/Login/IndexRetrun", "", function (data2) {//修改登录信息等
                    if (data2.result == "Success") {
                        var currToken = $.cookie('token'); //加入hear中的 token参数为了jwt ，在异步调用webapi时使用
                        $.ajaxSetup({
                            headers: { "Authorization": "Bearer " + currToken }
                        });
                        window.location.href = "/Home/Index";
                    }
                       
                }, "json");
            } else {
                layer.tips(data.message, "#btnLogin");
            };
        }
    })
};