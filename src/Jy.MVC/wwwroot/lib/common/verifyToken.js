//ie11下jquery cookie问题
jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie 
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE 
        }
        var path = options.path ? '; path=' + options.path : '';
        var domain = options.domain ? '; domain=' + options.domain : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie 
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want? 
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

//重写$.ajax方法
(function ($) {
    //首先备份下jquery的ajax方法  
    var _ajax = $.ajax;

    //重写jquery的ajax方法  
    $.ajax = function (opt) {
        var currToken = $.cookie('token'); //加入hear中的 token参数为了jwt ，在异步调用webapi时使用
        $.ajaxSetup({
            headers: { "Authorization": "Bearer " + currToken}
        });
        //备份opt中error和success方法  
        var fn = {
            error: function (XMLHttpRequest, textStatus, errorThrown) { },
            success: function (data, textStatus) { }
        }
        if (opt.error) {
            fn.error = opt.error;
        }
        if (opt.success) {
            fn.success = opt.success;
        }

        //扩展增强处理  
        var _opt = $.extend(opt, {
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //错误方法增强处理  
                fn.error(XMLHttpRequest, textStatus, errorThrown);
                if ("401"==(XMLHttpRequest.status))
                    window.location.href = "/Login/Index";
                if ("403" == (XMLHttpRequest.status)) {
                    alert("No Authority!");
                    window.location.href = "/Login/Index";
                }
                    
            },
            success: function (data, textStatus) {
                //成功回调方法增强处理  
                fn.success(data, textStatus);
            },
            beforeSend: function (XHR) {
                //提交前回调方法  
               // $('body').append("<div id='ajaxInfo' style=''>正在加载,请稍后...</div>");
            },
            complete: function (XHR, TS) {
                //请求完成后回调函数 (请求成功或失败之后均调用)。  
              //  $("#ajaxInfo").remove();;
            }
        });
        return _ajax(_opt);
    };
})(jQuery);


var currToken = $.cookie('token');
if (currToken != null || currToken != "")
    verifyToken();


function verifyToken() {
    var currToken = $.cookie("token");
    if (currToken == null || currToken == "")
        return;

    var postData = { "token": currToken };
    $.ajax({
        type: "Post",
        url: "/Login/VerifyToken",
        data: postData,
        success: function (data) {
           //debugger
            if (data.result == "Success") {
            } else {
                layer.tips(data.message);
            };
        }
    })
};
