var selectedId = "00000000-0000-0000-0000-000000000000";
$(function () {
    $("#btnUserRoleSave").click(function () { userRoleSave(); });
    $("#btnBatchUserRole").click(function () {

        var ids = "";
        $(".checkboxs").each(function () {
            if ($(this).prop("checked") == true) {
                ids += $(this).val() + "_"
            }
        });
        ids = ids.substring(0, ids.length - 1);
        if (ids.length == 0) {
            layer.alert("请选择要设置的记录。");
            return;
        };

        batchUserRoleIds = ids;
        $("#batchUserRoleModal").modal("show");
        initBatchUserRoleTree();
    });
    $("#btnBatchUserRoleSave").click(function () { batchUserRoleSave(); });
    $("#checkAll").click(function () { checkAll(this) });
    initTree();
});



//加载功能树
function initTree() {
    $.jstree.destroy('#treeDiv');
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


var roleIDs = "";
//加载功能树
function initUserRoleTree(id) {
   // $.jstree.destroy("#treeRoleDiv");
    $('#treeRoleDiv').jstree("destroy").empty();
    $.ajax({
        type: "Get",
        url: "/Authority/GetRoleTreeData?id=" + id + "&_t=" + new Date().getTime(),    //获取数据的ajax请求地址
        success: function (data) {
            var alreadycheckedList = new Array();
            var i = 0;
            $.each(data, function (idx, dataitem) {
                if (dataitem.checked == "1") {
                    alreadycheckedList[i] = dataitem;
                    i++;
                }
            });
            $('#treeRoleDiv').jstree({       //创建JsTtree
                'core': {
                    'data': data,        //绑定JsTree数据
                    "multiple": true    //是否多选
                },
                "plugins": ["checkbox", "state", "types", "wholerow"],  //配置信息
                "checkbox": {   //加入checkbox  
                    "keep_selected_style": false
                }
            })//------------end jstree
            $("#treeRoleDiv").on("ready.jstree", function (e, data) {   //树创建完成事件
                data.instance.open_all();    //展开所有节点
                data.instance.uncheck_all();
                data.instance.check_node(alreadycheckedList); //选中已经存在的数据项
            });//------------end on
            $("#treeRoleDiv").on('changed.jstree', function (e, data) {   //选中节点改变事件
                var nodes = data.instance.get_checked();  //使用get_checked方法
                if (nodes) {
                    roleIDs = "";
                    $.each(nodes, function (i, nd) {
                        if (!data.instance.is_parent(nd)) //非父节点
                            roleIDs = roleIDs + nd + "_";
                    });
                }
            });//------------end on
        }//------------end success
    });//------------end ajax
}//---------------end initTree


var roleBatchIDs = "";
//加载功能树
function initBatchUserRoleTree() {
    //$.jstree.destroy('#treeRoleBatchDiv');
    $('#treeRoleBatchDiv').jstree("destroy").empty();
    $.ajax({
        type: "Get",
        url: "/Authority/GetBatchRoleTreeData?_t=" + new Date().getTime(),    //获取数据的ajax请求地址
        success: function (data) {
            
            $('#treeRoleBatchDiv').jstree({       //创建JsTtree
                'core': {
                    'data': data,        //绑定JsTree数据
                    "multiple": true    //是否多选
                },
                "plugins": ["checkbox", "state", "types", "wholerow"],  //配置信息
                "checkbox": {   //加入checkbox  
                    "keep_selected_style": false
                }
            })//------------end jstree
            $("#treeRoleBatchDiv").on("ready.jstree", function (e, data) {   //树创建完成事件
                data.instance.open_all();    //展开所有节点
                data.instance.uncheck_all();
            });//------------end on
            $("#treeRoleBatchDiv").on('changed.jstree', function (e, data) {   //选中节点改变事件
                var nodes = data.instance.get_checked();  //使用get_checked方法
                if (nodes) {
                    roleBatchIDs = "";
                    $.each(nodes, function (i, nd) {
                        if (!data.instance.is_parent(nd)) //非父节点
                            roleBatchIDs = roleBatchIDs + nd + "_";
                    });
                }
            });//------------end on
        }//------------end success
    });//------------end ajax

}//---------------end initTree
//加载功能列表数据
function loadTables(startPage, pageSize) {
    $("#tableBody").html("");
    $("#checkAll").prop("checked", false);
    $.ajax({
        type: "GET",
        url: "/User/GetChildrenByParent?startPage=" + startPage + "&pageSize=" + pageSize + "&departmentId=" + selectedId + "&_t=" + new Date().getTime(),
        success: function (data) {
            $.each(data.rows, function (i, item) {

                var tr = "<tr>";
                tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
                tr += "<td>" + item.userName + "</td>";
                tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
                tr += "<td>" + (item.email == null ? "" : item.email) + "</td>";
                tr += "<td>" + (item.mobileNumber == null ? "" : item.mobileNumber) + "</td>";
                tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>";
                tr += "<td>" + (item.isDeleted == null ? "" : (item.isDeleted == 0 ? "有效" : "无效")) + "</td>";
                tr += "<td> <button class='btn btn-danger btn-xs' href='javascript:;' onclick='userRole(\"" + item.id + "\")'><i class='fa fa-circle-o'></i> 用户角色 </button></td>"
                tr += "</tr>";
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

//权限
function userRole(id) {
    $.ajax({
        type: "Get",
        url: "/User/Get?id=" + id + "&_t=" + new Date().getTime(),
        success: function (data) {
            $("#userRoleId").val(data.id);
            $("#userRoleTitle").text("设置权限");
            $("#userRoleModal").modal("show");
            initUserRoleTree(id);
        }
    });

};
//权限功能保存
function userRoleSave() {
    roleIDs = roleIDs.substring(0, roleIDs.length - 1);
    var postData = { "rpm": { "userRoleId": $("#userRoleId").val(), "roleIDs": roleIDs } };
    $.ajax({
        type: "Post",
        url: "/Authority/UserRole",
        data: postData,
        success: function (data) {
           // debugger
            if (data.result == "Success") {
                $("#userRoleModal").modal("hide");
                delayLoad("initTree()");
            } else {
                layer.tips(data.message, "#btnUserRoleSave");
            };
        }
    });
};

//批量
function batchUserRoleSave() {
    
    if (roleBatchIDs.length == 0) {
        layer.alert("请选择要设置的功能。");
        return;
    };
   
    //询问框
    layer.confirm("您确认设置选定的记录吗？", {
        btn: ["确定", "取消"]
    }, function () {
        roleBatchIDs = roleBatchIDs.substring(0, roleBatchIDs.length - 1);
        var postData = { "rpm": { "userIDs": batchUserRoleIds, "roleIDs": roleBatchIDs } };
        $.ajax({
            type: "Post",
            url: "/Authority/BatchUserRole",
            data: postData,
            success: function (data) {
              //  debugger
                if (data.result == "Success") {
                    $("#batchUserRoleModal").modal("hide");
                    layer.closeAll('dialog');
                    delayLoad("initTree()");
                } else {
                    layer.tips(data.message, "#btnBatchUserRoleSave");
                };
            }
        });
      
    });//----------------end  confirm
}; //----------------end  batchUserRoleSave



