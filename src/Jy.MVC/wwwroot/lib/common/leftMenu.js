function loadLeftMenu() {
    var currToken = $.cookie('token'); 
    $.ajax({
        type: "GET",
        url: "/Login/GetMenuForLeftMenu?token=" + currToken + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#leftMenu").empty();
            $("#leftMenu").append("<li class=\"header\">权限管理</li>");
            $.each(data.menus, function (i, item) {
                var tr = "<li><a href=\"" + item.url + "\"><i class=\"fa fa-link\"></i> <span>" + item.menuName + "</span></a></li>";
                $("#leftMenu").append(tr);
            });
            $("#unameh").html(data.userName);
            $("#unamel").html(data.userName);
            $("#deptname").html(data.departmentName);
            $("#email").html(data.email);
            $("#mobile").html(data.mobile);
            $("#useranddeptname").html(data.name + " - " + data.departmentName);
            $("#logintime").html(data.loginTime);
            
        }
    })
}
 
loadLeftMenu();