var selectedId = "00000000-0000-0000-0000-000000000000";
$(function () {
    $("#btnAdd").click(function () { add(1); });
    $("#btnSave").click(function () { save(); });
    $("#btnResetPassworSave").click(function () { resetPasswordSave(); });
    
    $("#btnDelete").click(function () { deleteMulti(); });
    $("#checkAll").click(function () { checkAll(this) });
    initTree();
});



//加载功能树
function initTree() {
    $.jstree.destroy("#treeDiv");
    $.ajax({
        type: "Get",
        url: "/User/GetTreeData?_t=" + new Date().getTime(),    //获取数据的ajax请求地址
        success: function (data) {
            $('#treeDiv').jstree({       //创建JsTtree
                'core': {
                    'data': data,        //绑定JsTree数据
                    "multiple": false    //是否多选
                },
                "plugins": ["state", "types", "wholerow"]  //配置信息
            })
            $("#treeDiv").on("ready.jstree", function (e, data) {   //树创建完成事件
                data.instance.open_all();    //展开所有节点
            });
            $("#treeDiv").on('changed.jstree', function (e, data) {   //选中节点改变事件
                var node = data.instance.get_node(data.selected[0]);  //获取选中的节点
                if (node) {
                    selectedId = node.id;
                    loadTables(1, 10);
                };
            });
        }
    });

}

function buildOneRow(item) {
    var lastlogintime = new Date(item.lastLoginTime).Format("yyyy-MM-dd HH:mm:ss");
    var createTime = new Date(item.createTime).Format("yyyy-MM-dd HH:mm:ss");
    var tr = "<tr>";
    tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
    tr += "<td>" + item.userName + "</td>";
    tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
    tr += "<td>" + (item.email == null ? "" : item.email) + "</td>";
    tr += "<td>" + (item.mobileNumber == null ? "" : item.mobileNumber) + "</td>";
    tr += "<td>" + (lastlogintime == null ? "" : lastlogintime) + "</td>";
    tr += "<td>" + (item.loginTimes == null ? "" : item.loginTimes) + "</td>";
    tr += "<td>" + (item.createUserName == null ? "" : item.createUserName) + "</td>";
    tr += "<td>" + (createTime == null ? "" : createTime) + "</td>";
    tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>";
    tr += "<td>" + (item.isDeleted == null ? "" : (item.isDeleted == 0 ? "有效" : "无效")) + "</td>";
    tr += "<td><button class='btn btn-info btn-xs' href='javascript:;' onclick='edit(\"" + item.id + "\")'><i class='fa fa-edit'></i> 编辑 </button> <button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button> <button class='btn btn-danger btn-xs' href='javascript:;' onclick='resetPassword(\"" + item.id + "\")'><i class='fa fa-circle-o'></i> 重置密码 </button></td>"
    tr += "</tr>";
    return tr;
}
//加载功能列表数据
function loadTables(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: spBackURL +"/User/GetChildrenByParent?startPage=" + startPage + "&pageSize=" + pageSize + "&departmentId=" + selectedId + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {
                var tr = buildOneRow(item);
                $("#tableBody").append(tr);
            })
            var elment = $("#grid_paging_part"); //分页插件的容器id
            if (data.rowCount > 0) {
                var options = { //分页插件配置项
                    bootstrapMajorVersion: 3,
                    currentPage: startPage, //当前页
                    numberOfPages: data.rowsCount, //总数
                    totalPages: data.pageCount, //总页数
                    onPageChanged: function (event, oldPage, newPage) { //页面切换事件
                        loadTables(newPage, pageSize);
                    }
                }
                elment.bootstrapPaginator(options); //分页插件初始化
            }
        }
    })
}
//全选
function checkAll(obj) {
    $(".checkboxs").each(function () {
        if (obj.checked == true) {
            $(this).prop("checked", true)

        }
        if (obj.checked == false) {
            $(this).prop("checked", false)
        }
    });
};
//新增
function add(type) {
    $("#DepartmentId").val(selectedId);
    $("#Title").text("新增");
    $("#Id").val("00000000-0000-0000-0000-000000000000");
    $("#UserName").val("");
    $("#Name").val("");
    $("#Password").val(""); $("#PasswordDiv").show();
    $("#Email").val("");
    $("#MobileNumber").val("");
    $("#Remarks").val("");
    //弹出新增窗体
    $("#addRootModal").modal("show");
};
//编辑
function edit(id) {
    $.ajax({
        type: "Get",
        url: spBackURL +"/User/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#Id").val(data.id);
            //$("#Password").val(data.password);
            $("#PasswordDiv").hide();
            $("#DepartmentId").val(data.departmentId);
            $("#Name").val(data.name);
            $("#UserName").val(data.userName);
            $("#Email").val(data.email);
            $("#MobileNumber").val(data.mobileNumber);
            $("#Remarks").val(data.remarks);

            $("#Title").text("编辑功能")
            $("#addRootModal").modal("show");
        }
    })
};
//保存
function save() {
    var postData = { "dto": { "Id": $("#Id").val(), "DepartmentId": $("#DepartmentId").val(), "Name": $("#Name").val(), "UserName": $("#UserName").val(), "Email": $("#Email").val(), "MobileNumber": $("#MobileNumber").val(), "Remarks": $("#Remarks").val(), "Password": $("#Password").val() } };
    $.ajax({
        type: "Post",
        url: spBackURL + "/User/Edit",
        data: postData,
        success: function (data) {
            //debugger
            if (data.result == "Success")  {
                $("#addRootModal").modal("hide");
                delayLoad("initTree()");
            } else {
                layer.tips(data.message, "#btnSave");
            };
        }
    })
};
//重置密码
function resetPassword(id) {
    $.ajax({
        type: "Get",
        url: spBackURL +"/User/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#resetPasswordId").val(data.id);
            $("#resetPasswordUserName").text(data.userName);
            $("#OldPassword").val("");
            $("#NewPassword").val("");
            $("#NewPassword2").val("");
            $("#resetPasswordTitle").text("重置密码");
            $("#resetPasswordModal").modal("show");
        }
    })
};
//重置密码保存
function resetPasswordSave() {
    var postData = { "rpm": { "resetPasswordId": $("#resetPasswordId").val(), "OldPassword": $("#OldPassword").val(), "NewPassword": $("#NewPassword").val(), "NewPassword2": $("#NewPassword2").val() } };
    $.ajax({
        type: "Post",
        url: "/User/ResetPassword",
        data: postData,
        success: function (data) {
            //debugger
            if (data.result == "Success") {
                $("#resetPasswordModal").modal("hide");
                delayLoad("initTree()");
            } else {
                layer.tips(data.message, "#btnResetPassworSave");
            };
        }
    })
};
//批量删除
function deleteMulti() {
    var ids = "";
    $(".checkboxs").each(function () {
        if ($(this).prop("checked") == true) {
            ids += $(this).val() + ","
        }
    });
    ids = ids.substring(0, ids.length - 1);
    if (ids.length == 0) {
        layer.alert("请选择要删除的记录。");
        return;
    };
    //询问框
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        var sendData = { "ids": ids };
        $.ajax({
            type: "Post",
            url: spBackURL +"/User/DeleteMuti",
            data: sendData,
            success: function (data) {
                if (data.result == "Success") {
                    layer.closeAll();
                    delayLoad("initTree()");
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        });
    });
};

//删除单条数据
function deleteSingle(id) {
    layer.confirm("您确认删除选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        $.ajax({
            type: "POST",
            url: spBackURL +"/User/Delete",
            data: { "id": id },
            success: function (data) {
                if (data.result == "Success") {
                    layer.closeAll();
                    delayLoad("initTree()");
                }
                else {
                    layer.alert("删除失败！");
                }
            }
        })
    });
};


