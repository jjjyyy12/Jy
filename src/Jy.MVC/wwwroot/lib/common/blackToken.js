
function blackToken() {
    var currToken = $.cookie("token");
    if (currToken == null || currToken == "")
        return;

    var postData = { "token": currToken };
    $.ajax({
        type: "Post",
        url: "/Login/LogOutToken",
        data: postData,
        success: function (data) {
           //debugger
            if (data.result == "Success") {
                $.cookie("token", null); 
                window.location.href = "/Login/Index";
            } else {
            };
        }
    })
};
