var selectedId = "00000000-0000-0000-0000-000000000000";
$(function () {
    $("#btnAdd").click(function () { add(1); });
    $("#btnSave").click(function () { save(); });
    $("#btnRoleMenuSave").click(function () { roleMenuSave(); });
    
    $("#btnDelete").click(function () { deleteMulti(); });
    $("#checkAll").click(function () { checkAll(this) });
    initTree();
});
function initTree() {
    loadTables(1, 10);
}

var menuIDs = "";
var checkedList = new Array();
//加载功能树
function initRoleMenuTree(id) {
   // $.jstree.destroy("#treeDiv");
    $('#treeDiv').jstree("destroy").empty();
    $.ajax({
        type: "Get",
        url: "/Role/GetMenuTreeData?id="+id+"&_t=" + new Date().getTime(),    //获取数据的ajax请求地址
        success: function (data) {
            var alreadycheckedList = new Array();
            var i = 0;
            $.each(data, function (idx,dataitem) {
                if (dataitem.checked == "1") {
                    alreadycheckedList[i] = dataitem;
                    i++;
                }
            });
            $('#treeDiv').jstree({       //创建JsTtree
                'core': {
                    'data': data,        //绑定JsTree数据
                    "multiple": true    //是否多选
                },
                "plugins": ["checkbox","state", "types", "wholerow"],  //配置信息
                "checkbox": {   //加入checkbox  
                    "keep_selected_style": false
                }
            })//------------end jstree
            $("#treeDiv").on("ready.jstree", function (e, data) {   //树创建完成事件
                data.instance.open_all();    //展开所有节点
                data.instance.uncheck_all(); 
                data.instance.check_node(alreadycheckedList); //选中已经存在的数据项
            });//------------end on
            $("#treeDiv").on('changed.jstree', function (e, data) {   //选中节点改变事件
                //var node = data.instance.get_node(data.selected[0]);  //获取选中的节点
                //if (node) {
                //    selectedId = node.id;
                //    // loadTables(1, 10);
                //};
                //var ref = $('#treeDiv').jstree(true);.
                var nodes = data.instance.get_checked();  //使用get_checked方法
                if (nodes){
                    menuIDs = "";
                    $.each(nodes, function (i, nd) {0
                       // if(!data.instance.is_parent(nd)) //非父节点
                            menuIDs = menuIDs + nd + "_";
                    });
                }
            });//------------end on
        }//------------end success
    });//------------end ajax
}//---------------end initRoleMenuTree

    //加载功能列表数据
    function loadTables(startPage, pageSize) {
        $("#tableBody").html("");
        $("#checkAll").prop("checked", false);
        $.ajax({
            type: "GET",
            url: "/Role/GetListPaged?startPage=" + startPage + "&pageSize=" + pageSize + "&_t=" + new Date().getTime(),
            success: function (data) {
                $.each(data.rows, function (i, item) {
                    var createTime = new Date(item.createTime).Format("yyyy-MM-dd HH:mm:ss");
                    var tr = "<tr>";
                    tr += "<td align='center'><input type='checkbox' class='checkboxs' value='" + item.id + "'/></td>";
                    tr += "<td>" + (item.code == null ? "" : item.code) + "</td>";
                    tr += "<td>" + (item.name == null ? "" : item.name) + "</td>";
                    tr += "<td>" + (item.createUserName == null ? "" : item.createUserName) + "</td>";
                    tr += "<td>" + (createTime == null ? "" : createTime) + "</td>";
                    tr += "<td>" + (item.remarks == null ? "" : item.remarks) + "</td>";
                    tr += "<td><button class='btn btn-info btn-xs' href='javascript:;' onclick='edit(\"" + item.id + "\")'><i class='fa fa-edit'></i> 编辑 </button> <button class='btn btn-danger btn-xs' href='javascript:;' onclick='deleteSingle(\"" + item.id + "\")'><i class='fa fa-trash-o'></i> 删除 </button> <button class='btn btn-danger btn-xs' href='javascript:;' onclick='roleMenu(\"" + item.id + "\")'><i class='fa fa-circle-o'></i> 角色功能 </button></td>"
                    tr += "</tr>";
                    $("#tableBody").append(tr);
                })
                var elment = $("#grid_paging_part"); //分页插件的容器id
                if (data.rowCount > 0) {
                    var options = { //分页插件配置项
                        bootstrapMajorVersion: 3,
                        currentPage: startPage, //当前页d
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
        $("#Code").val("");
        $("#Name").val("");
        $("#Remarks").val("");
        //弹出新增窗体
        $("#addRootModal").modal("show");
    };
    //编辑
    function edit(id) {
        $.ajax({
            type: "Get",
            url: "/Role/Get?id=" + id + "&_t=" + new Date().getTime(),
            success: function (data) {
                $("#Id").val(data.id);
                $("#Code").val(data.code);
                $("#Name").val(data.name);
                $("#Remarks").val(data.remarks);

                $("#Title").text("编辑功能")
                $("#addRootModal").modal("show");
            }
        })
    };
    //保存
    function save() {
        var postData = { "dto": { "Id": $("#Id").val(), "Code": $("#Code").val(), "Name": $("#Name").val(), "Remarks": $("#Remarks").val() } };
        $.ajax({
            type: "Post",
            url: "/Role/Edit",
            data: postData,
            success: function (data) {
                //debugger
                if (data.result == "Success") {
                    $("#addRootModal").modal("hide");
                    delayLoad("initTree()");
                } else {
                    layer.tips(data.message, "#btnSave");
                };
            }
        });
    };


    function roleMenu(id) {
        $.ajax({
            type: "Get",
            url: "/Role/Get?id=" + id + "&_t=" + new Date().getTime(),
            success: function (data) {
                $("#roleMenuId").val(data.id);
                $("#roleMenuTitle").text("设置功能");
                $("#roleMenuModal").modal("show");
                initRoleMenuTree(id);
            }
        });

    };
    //权限功能保存
    function roleMenuSave() {
        menuIDs = menuIDs.substring(0, menuIDs.length - 1);
        var postData = { "rpm": { "roleMenuId": $("#roleMenuId").val(), "menuIDs": menuIDs } };
        $.ajax({
            type: "Post",
            url: "/Role/RoleMenu",
            data: postData,
            success: function (data) {
               // debugger
                if (data.result == "Success") {
                    $("#roleMenuModal").modal("hide");
                    delayLoad("initTree()");
                } else {
                    layer.tips(data.message, "#btnRoleMenuSave");
                };
            }
        });
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
                url: "/Role/DeleteMuti",
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
                url: "/Role/Delete",
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


