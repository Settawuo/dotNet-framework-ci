
function showDialog(msg) {
    $("#windowMessage2").html(msg + '<br><center><a onclick=\"closeDialog()\" class=\"k-button\" style=\"text-decoration:none;padding:1px 20px;margin-top:25px;\">OK</a></center>');
    $("#windowMessage").data("kendoWindow").center().open();
}

function closeDialog() {
    $("#windowMessage").data("kendoWindow").close();
    //e.focus();
}

function showDialogErrorMessage(panel_id, html_list_err_msg, msg) {
    //debugger;
    //$("#windowMessage2").html(msg + '<br><center><a onclick=\"closeDialog()\" class=\"k-button\" style=\"text-decoration:none;padding:1px 20px;margin-top:25px;\">OK</a></center>');
    //$("#windowMessage").data("kendoWindow").center().open();
    //$.error(msg);
    
    showFeedback("error", msg);
    if (html_list_err_msg != undefined && html_list_err_msg.length > 0 && panel_id != undefined) {
        var html_li = "";
        for (var i = 0; i < html_list_err_msg.length; i++) {
            html_li += "<li>" + html_list_err_msg[i] + "</li>";
        }
        $("#" + panel_id + " .error_details").html("<div class='alert alert-danger alert-dismissable'><ul>" + html_li + "</ul></div>");
    }

}

function showPanelErrorMessage(panel_id, html_list_err_msg) {
    if (html_list_err_msg != undefined && html_list_err_msg.length > 0 && panel_id != undefined) {
        var html_li = "";
        for (var i = 0; i < html_list_err_msg.length; i++) {
            html_li += "<li>" + html_list_err_msg[i] + "</li>";
        }
        $("#" + panel_id + " .error_details").html("<div class='alert alert-danger alert-dismissable'><ul>" + html_li + "</ul></div>");
    }
}
function clearPanelErrorMessage(panel_id) {
    $("#" + panel_id + " .error_details").html("");
}
function clearAllPanelErrorMessage() {
    $(" .error_details").html("");
}
function addExtensionClass(extension) {
    switch (extension) {
        case '.jpg':
            return "jpg-file";
        case '.png':
        case '.img':
        case '.gif':
            return "png-file";
        case '.doc':
        case '.docx':
            return "doc-file";
        case '.xls':
        case '.xlsx':
            return "xls-file";
        case '.pdf':
            return "pdf-file";
        case '.zip':
        case '.rar':
            return "zip-file";
        default:
            return "default-file";
    }
}

function returnClass(panel, e) {
    if (e.className == "toggle-up") {
        $(panel).removeClass(e.className);
        $(panel).addClass("toggle-down");
    }
    else {
        $(panel).removeClass(e.className);
        $(panel).addClass("toggle-up");
    }
}

function toggle(e) {
    var panel = "#"
    var id = e.id.split('_');
    if (id[1] != undefined)
        panel += id[1];
    $(panel).toggle();
    panel = "#_" + id[1];
    returnClass(panel, e);
}

function validateEmail(str) {
    var index_at = str.indexOf('@')
    if (index_at == -1) {
        return false;
    }

    var name = str.substr(0, index_at);
    /* should test name for other invalids*/

    var domain = str.substr(index_at + 1);
    /* should check for extra "@" and any other checks that would invalidate an address of which there are likely many*/
    if (domain.indexOf('@') != -1) {
        return false;
    }
    return domain.indexOf('.') > 1;
}
function SetPermission(commponentName, isEnable, isReadOnly, tabDetail) {
    //alert("1123");
    //debugger;
    var element = $("div[id='" + tabDetail + "']").find("[permission*='" + commponentName + "']");
    //var element = $("[permission*='" + commponentName + "']");
    if (element.length > 0) {

        element.each(function (index, elem) {
            //debugger
            var jqueryElement = $("#" + elem.id);
            if (jqueryElement.length > 0) {
                //debugger
                //alert(jqueryElement[0].dataset);
                if (jqueryElement.data("kendoNumericTextBox") != undefined && jqueryElement.data("kendoNumericTextBox") != null) {
                    if (isEnable == "False") {
                        //hide
                        jqueryElement.data("kendoNumericTextBox").wrapper.hide();
                    }
                    else {
                        if (isReadOnly == "True") {
                            //disabled
                            jqueryElement.data("kendoNumericTextBox").enable(false);
                        }
                    }
                }
                else if (jqueryElement.data("kendoDropdownlist") != undefined && jqueryElement.data("kendoDropdownlist") != null) {
                    if (isEnable == "False") {
                        //hide
                        jqueryElement.data("kendoDropdownlist").wrapper.hide();
                    }
                    else {
                        if (isReadOnly == "True") {
                            //disabled
                            jqueryElement.data("kendoDropdownlist").enable(false);
                        }
                    }
                }

                else {
                    if (isEnable == "False") {
                        jqueryElement.hide();
                    }
                }
            }
        });
    }
}

function ValidateSavePanel(validateModel) {
    var i = 0;
    var result = true;
    for (i = 0; i < validateModel.length; i++) {
        
        var id = validateModel[i].id;
        var name = validateModel[i].name;
        var isRequired = validateModel[i].required;
        var value = "";
        //debugger

        if ($("#" + id).closest('.k-dropdown') != undefined &&
                $("#" + id).closest('.k-dropdown').length > 0) {
            value = $("#" + id).data('kendoDropDownList').value();
        } else {
            value = $("#" + id).val();
        }

        if (isRequired && $.trim(value) == "") {
            $("#validatefor-" + id).text(name + " is required.");
            $("#validatefor-" + id).css("font-size", "80%");

            if ($("#" + id).closest('.k-dropdown') != undefined &&
                $("#" + id).closest('.k-dropdown').length > 0) {
                $("#" + id).closest('.k-dropdown').addClass("input-validation-error");
            } else if ($("#" + id).closest('.k-numerictextbox') != undefined &&
                $("#" + id).closest('.k-numerictextbox').length > 0) {
                $("#" + id).closest('.k-numerictextbox').addClass("input-validation-error");
            } else {
                $("#" + id).addClass("input-validation-error");
            }

            //if ($("#" + id)[0].dataset.role == "numerictextbox" ||
            //    $("#" + id)[0].dataset.role == "dropdownlist") {

            //    // $("#" + id).parent().attr("style", "border: 1px solid #e80c4d");
            //}
            result = false;
        }
        else
            $("#validatefor-" + id).text('');
    }
    return result;
}

function OnRowIconDataBound(e) {
    //debugger
    e.sender.element.find(".k-button.k-button-icontext.k-grid-View").find("span").addClass("fa fa-eye");
    e.sender.element.find(".k-button.k-button-icontext.k-grid-View")
        .removeClass("k-button k-button-icontext").addClass("btn btn-default btn-sm i-gap-right-xs view-only");

    //e.sender.element.find(".k-grid-View").find("span").addClass("k-icon k-i-restore");
    //e.sender.element.find(".k-grid-View").find("span").parent().addClass("tooltip");
    //e.sender.element.find(".k-grid-View").find("span").parent().attr("title", "View");

    //e.sender.element.find(".k-button.k-button-icontext.k-grid-Edit").each(function (i, e) {
    //    $('#' + this.id).removeClass("k-button").addClass("k-icon k-edit");
    //});

    e.sender.element.find(".k-button.k-button-icontext.k-grid-Edit").find("span").addClass("fa fa-pencil");
    e.sender.element.find(".k-button.k-button-icontext.k-grid-Edit")
        .removeClass("k-button k-button-icontext").addClass("btn btn-default btn-sm i-gap-right-xs");

    e.sender.element.find(".k-button.k-button-icontext.k-grid-BA").find("span").addClass("fa fa-plus");
    e.sender.element.find(".k-button.k-button-icontext.k-grid-BA")
        .removeClass("k-button k-button-icontext").addClass("btn btn-default btn-sm i-gap-right-xs");
    //e.sender.element.find(".k-button.k-button-icontext.k-grid-Edit").find("span").addClass("k-icon k-edit");
    //e.sender.element.find(".k-grid-Edit").find("span").parent().addClass("tooltip");
    //e.sender.element.find(".k-grid-Edit").find("span").parent().attr("title", "Edit");

    e.sender.element.find(".k-button.k-button-icontext.k-grid-Delete").find("span").addClass("fa fa-trash-o");
    e.sender.element.find(".k-button.k-button-icontext.k-grid-Delete")
        .removeClass("k-button k-button-icontext").addClass("btn btn-default btn-sm i-gap-right-xs");

    //e.sender.element.find(".k-grid-Delete").find("span").addClass("k-icon k-cancel");
    //e.sender.element.find(".k-grid-Delete").find("span").parent().addClass("tooltip");
    //e.sender.element.find(".k-grid-Delete").find("span").parent().attr("title", "Delete");

    e.sender.element.find(".k-grid-MoreDetail").find("span").addClass("k-icon k-i-restore");
    //e.sender.element.find(".k-grid-MoreDetail").find("span").parent().addClass("tooltip");
    //e.sender.element.find(".k-grid-MoreDetail").find("span").parent().attr("title", "More Detail");

    e.sender.element.find(".k-grid-History").find("span").addClass("fa fa-clock-o");
    e.sender.element.find(".k-button.k-button-icontext.k-grid-History")
        .removeClass("k-button k-button-icontext").addClass("btn btn-default btn-sm i-gap-right-xs view-only");
    //e.sender.element.find(".k-grid-History").find("span").parent().addClass("tooltip");
    //e.sender.element.find(".k-grid-History").find("span").parent().attr("title", "History");

    e.sender.element.find(".k-grid-NewBA").find("span").addClass("k-icon k-i-plus");
    //e.sender.element.find(".k-grid-NewBA").find("span").parent().addClass("tooltip");
    //e.sender.element.find(".k-grid-NewBA").find("span").parent().attr("title", "New BA");

    e.sender.element.find(".k-grid-Select").find("span").addClass("fa fa-check-square-o");
    e.sender.element.find(".k-button.k-button-icontext.k-grid-Select")
        .removeClass("k-button k-button-icontext").addClass("btn btn-default btn-sm i-gap-right-xs");
    //e.sender.element.find(".k-grid-Select").find("span").parent().addClass("tooltip");
    //e.sender.element.find(".k-grid-Select").find("span").parent().attr("title", "Select");

    $('.tooltip').tooltipster({
        animation: 'fade',
        delay: 200,
        theme: 'tooltipster-punk',
        touchDevices: false,
        trigger: 'hover'
    });
}


function IwfProgress() {

    var panelbodypage = $("#panelbodypage");

    this.Load = function () {
        kendo.ui.progress(panelbodypage, true);
    }

    this.LoadComplete = function () {
        kendo.ui.progress(panelbodypage, false);
    }
}

function PanelProgress(panelName) {

    var panelbodypage = $("#" + panelName + "");

    this.Load = function () {
        kendo.ui.progress(panelbodypage, true);
    }

    this.LoadComplete = function () {
        kendo.ui.progress(panelbodypage, false);
    }
}

/** script upload file **/
/* created by thanaprn 11/02/2014*/
var uploadFile = new UploadFileTemplate();

function UploadFileTemplate() {
    this.index = function (dataItem, gridID) {
        var data = $("#" + gridID).data("kendoGrid").dataSource.data();

        return data.indexOf(dataItem);
    }
    this.allowExtension = function (extension) {
        //switch (extension) {
        //    case '.jpg':
        //    case '.png':
        //    case '.gif':
        //    case '.doc':
        //    case '.docx':
        //    case '.xls':
        //    case '.xlsx':
        //    case '.pdf':
        //        return true;
        //    default:
        //        return false;
        //}
        return true;
    }
    this.onComplete = function (e) {
        //grid refresh
        //debugger
        var id_upload = this.element.attr("id");
        $("#grid_" + id_upload).data("kendoGrid").dataSource.read();
        $("#grid_" + id_upload).find("ul[class='k-upload-files k-reset']").remove();
        $("#grid_" + id_upload).find("button[class='k-button k-upload-selected']").remove();
        $("#grid_" + id_upload).find("input[type='file']")
        //showDialog("Upload Complete");
    }
    this.onError = function (e) {
        ////////console.log(e);
        ////////debugger;
        //////if (e.files.length > 0) {
        //////    //debugger
        //////    var filename = e.files[0].name;
        //////    //alert("");
        //////    //$.warning("File " + filename + " upload not support!");
        //////    //$.warning("File " + filename + " upload not support!");
            
        //////    showFeedback("warning", "ไม่สามารถ Upload file นี้ได้ เนื่องจากไฟล์เสีย,แนบไฟล์ไม่ถูกต้อง หรือไฟล์มีขนาดใหญ่เกินไป");
        //////    var id_upload = this.element.attr("id");

        //////    $("#grid_" + id_upload).find("li").find("div").each(function (index, element) {
        //////        if ($(element).attr("filename") == filename) {
        //////            $(element).parent().remove();
        //////        }
        //////    });
        //////    $("#grid_" + id_upload).find("strong").remove();

        //////}//$.warning(e.response.message);
    }
    this.onUpload = function (e) {
        //var files = e.files;
        //var waiting_process = "wait";
        ////debugger
        //$.each(files, function () {
        //    //debugger
        //    var extension = this.extension.toLowerCase();
        //    var size = this.size;
        //    var eObject = e;
        //    $.ajax({
        //        type: "POST",
        //        url: "/Upload/IsAllowFile",
        //        data: "{type:'" + extension + "',size:"+size+"}",
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (response) {
        //            debugger
        //            var message_response = eval(response);
        //            if (message_response.response) {
        //                //$("#" + grid_id).data("kendoGrid").dataSource.read();
        //                //reload();
        //                waiting_process = "done";
        //            }
        //            else {
        //                //showDialog("ไม่สามารถลบข้อมูลได้");
        //                waiting_process = "done";
        //                alert(message_response.message);
        //                eObject.preventDefault();
        //            }

        //        },
        //        failure: function (msg) {
        //            //showDialog("ไม่สามารถลบข้อมูลได้");
        //            //$("#" + grid_id).data("kendoGrid").dataSource.read();

        //            waiting_process = "done";
        //            eObject.preventDefault();
        //        }
        //    });


        //    //if (!uploadFile.allowExtension(this.extension.toLowerCase())) {
        //        //showDialog("File upload not support!")
        //        //e.preventDefault();
        //    //}

        //    //else if (this.size / 1024 / 1024 > 5) {
        //        //showDialog("Max 5Mb file size is allowed!")
        //        //e.preventDefault();
        //    //}
        //});

        //var process_id = setInterval(function () {
        //    debugger;
        //    if (waiting_process != "wait")
        //    {
        //        clearInterval(process_id);
        //    }
        //}, 1000);

    }

    this.onSelectUpload = function (e) {
        //debugger
        var grid_id = this.element.attr("id");
        if ($("#" + grid_id).find("ul[class='k-upload-files k-reset']").length == 0) {
            //send remove session
        }
    }
    this.onSuccess = function (e) {
        //e.preventDefault();
        //debugger
        if (e.response.status != undefined) {
            if (!e.response.status) {
                //$.warning(e.response.message);
                showFeedback("warning", e.response.message);
                //alert(e.response.message);
                //debugger
                var filename = e.response.filename;
                var id_upload = this.element.attr("id");

                $("#grid_" + id_upload).find("li").find("div").each(function (index, e) {
                    //debugger
                    if ($(e).attr("filename") == filename) {
                        $(e).parent().remove();
                    }
                });
                $("#grid_" + id_upload).find("strong").remove();
            }
        }
    }

    this.onUploadFileDelete = function (e) {
        if (confirm("ต้องการลบข้อมูลไฟล์นี้ ?")) {
            e.preventDefault();

            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            //alert(dataItem.ID);
            var grid_id = this.element.attr("id");

            $.ajax({
                type: "POST",
                url: "/Upload/Delete",
                data: "{data:" + dataItem.ID + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    //debugger
                    if (response) {
                        $("#" + grid_id).data("kendoGrid").dataSource.read();
                        //reload();
                    }
                    else {
                        //$.error("ไม่สามารถลบข้อมูลได้");
                        showFeedback("error", "ไม่สามารถลบข้อมูลได้");
                    }

                },
                failure: function (msg) {
                    //$.error("ไม่สามารถลบข้อมูลได้");
                    showFeedback("error", "ไม่สามารถลบข้อมูลได้");
                    $("#" + grid_id).data("kendoGrid").dataSource.read();
                }
            });

            //$(this.element.attr("id")).data("kendoGrid");

            //var item = $("#"+this.element.attr("id")).data("kendoGrid").dataItem($(this).closest("tr"));
            //// item contains the item corresponding to clicked row
            //alert(item);
            //// If I want to remove the row...
            //$("#" + this.element.attr("id")).data("kendoGrid").removeRow($(this).closest("tr"));
            //$("#grid_" + id_upload).data("kendoGrid").dataSource.read();
        }
    }
}
/** end script upload **/
/** download file uri encode **/
function downloadFileByID(id, type) {
    $.fileDownload('/Workflows/DownloadFileID?id=' + id + '&type=' + type)
            .done(function () {
                //alert('File download a success!');
            })
            .fail(function () {
                alert("File not found can't download!");
            });
}
/*download file ใช้ตอน stage 1 service info*/
function downloadFile(filename, type) {
    //debugger
    //alert(filename + " " + type);
    //alert(encodeURIComponent(filename) + " " + type);
    //window.location = '/Workflows/DownloadFile?filename=' + encodeURIComponent(filename) + '&type=' + type;

    //$.fileDownload($(this).prop('href'), {
    //    preparingMessageHtml: "We are preparing your report, please wait...",
    //    failMessageHtml: "There was a problem generating your report, please try again."
    //});
    //return false;

    //this is critical to stop the click event which will trigger a normal file download!
    //alert('/Workflows/DownloadFile?filename=' + encodeURIComponent(filename) + '&type=' + type);
    //try{
        $.fileDownload('/Workflows/DownloadFile?filename=' + encodeURI(filename) + '&type=' + type)
            .done(function () {
                //alert('File download a success!');
            })
            .fail(function () {
                alert("File not found can't download!");
            });
    //}
    //catch (ex)
    //{
        //window.location = '/Workflows/DownloadFile?filename=' + encodeURIComponent(filename) + '&type=' + type;
    //}
    //return false;
}

/* begin workspace helper class */

function avoidNull(text) {
    try {
        if (text != null && text != -1 && text != '-1')
            return text;
    }
    catch (ex) {
        return "-";
    }
    return "-";
}
/* end workspace helper class */

var Loading;
Loading = Loading || (function () {
    return {
        show: function () {
            
            bootbox.dialog({
                //message: "<div class='modal-body'><div class='progress progress-striped active'><div class='bar' style='width: 100%;'></div></div></div>",
                message: "<div class='modal-body'> <div class='iwf-loading'> </div> </div>",
                title: "<i class='fa fa-spinner'></i> Processing...",
                closeButton: false
            });
        },
        hide: function () {
            bootbox.hideAll();
        },
    };
})();