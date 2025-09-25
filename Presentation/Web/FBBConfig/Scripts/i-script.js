function ValidationIcon() {
    return "<i class='fa fa-warning i-red'></i> ";
}

function onPanelToggle(e) {
    var id = $("#" + e.id + "Arrow");
    var cls = id.attr("class");
    if (cls === "fa fa-chevron-circle-down fa-lg") {
        id.removeClass();
        id.addClass("fa fa-chevron-circle-up fa-lg");
    }
    else {
        id.removeClass();
        id.addClass("fa fa-chevron-circle-down fa-lg");
    }
}

function clearPageScrollUp(idClass) {
    $(idClass + " a[href=#]").removeAttr("href");
}

function convertDate(dt) {
    try {
        if (dt == "" || dt == null) return null;
        else {
            var dtSplit = dt.split('/');
            return new Date(dtSplit[2], (dtSplit[1] - 1), dtSplit[0]);
        }
    }
    catch (ex) {
        return null;
    }
}

function showFeedback(type, msg, title) {
    //if (msg !== "") {
        $("#headerMessageFeedback").removeClass();
        $("#headerMessageFeedback").addClass("modal-header");
        if (type === "primary") { $("#headerMessageFeedback").addClass("btn-primary"); }
        else if (type === "success") { $("#headerMessageFeedback").addClass("btn-success");}
        else if (type === "info") { $("#headerMessageFeedback").addClass("btn-info");}
        else if (type === "warning") { $("#headerMessageFeedback").addClass("btn-warning");}
        else if (type === "error") { $("#headerMessageFeedback").addClass("btn-danger");}
        else { $("#headerMessageFeedback").addClass("btn-default"); }

        if (title !== undefined) $("#MessageFeedbackPopUpLabel").text(title); else $("#MessageFeedbackPopUpLabel").text("Notification");
        

        $("#lbMessageFeedback").text(msg);

        $("#MessageFeedbackPopUp").modal('show');
    //}
}

function SetRequired(validateModel) {
    var i = 0;
    for (i = 0; i < validateModel.length; i++) {
        var id = validateModel[i].id;
        $("#requiredfor-" + id).addClass("label label-danger i-gap-left-md i-required");
        $("#requiredfor-" + id).text("Required");
    }
}

function SetValidateLength(validateLengthModel) {
    var i = 0;
    for (i = 0; i < validateLengthModel.length; i++) {
        var id = validateLengthModel[i].id;
        var length = validateLengthModel[i].length;
        if (length !== undefined) {
            $("#validatefor-" + id).addClass("i-validate-length i-length-" + length);
        }
    }
}

function RemoveRequired(panel) {
    $("#" + panel + " span[id*=requiredfor-]").removeClass("label label-danger i-gap-left-md i-required");
    $("#" + panel + " span[id*=requiredfor-]").text("");
    ClearTextValidation(panel);
}

function ClearTextValidation(panel) {
    $("#" + panel + " .i-required").each(function (index, e) {
        var field = e.id.split('requiredfor-')[1];
        $("#validatefor-" + field).text("");
    });
}

function RemoveValidation(validateModel) {
    var i = 0;
    for (i = 0; i < validateModel.length; i++) {
        var id = validateModel[i].id;
        $("#requiredfor-" + id).removeClass("label label-danger i-gap-left-md i-required");
        $("#requiredfor-" + id).text("");
        $("#validatefor-" + id).html("");
    }
}

function ValidationProcessing(panel) {
    var chk = true;
    $("#" + panel + " .i-required").each(function (index, e) {        
        var field = e.id.split('requiredfor-')[1];
        var value = $("#" + field).val();
        if ($.trim(value) == "") {
            $("#validatefor-" + field).html(ValidationIcon() + "This field is required.");
            chk = false;
        }
        else {
            $("#validatefor-" + field).html("");
        }
    });

    if (chk) {
        $("#" + panel + " .i-validate-length").each(function (index, e) {
            var field = e.id.split('validatefor-')[1];
            var value = $("#" + field).val();
            var length = parseInt(e.className.split('i-length-')[1]);
            var val_length = 0;
            if (value != "") {
                val_length = value.length;
            }

            if (val_length > length) {
                $("#" + e.id).html(ValidationIcon() + "This maxlength field is " + length + ".");
                chk = false;
            }
            else {
                $("#" + e.id).html("");
            }
        });
    }

    return chk;
}


function Loading(x) {
    if (x == 0) {
        $("#PopupLoading").data("kendoWindow").close();
    }
    else {
        $("#PopupLoading").data("kendoWindow").open();
        $("#PopupLoading").data("kendoWindow").center();
    }
}

function setButton() {
    $(".k-grid-Delete").removeClass("k-button k-button-icontext").addClass("btn btn-default i-gap-right-xs-3").css("width", "65px");
    $(".k-grid-Edit").removeClass("k-button k-button-icontext").addClass("btn btn-default i-gap-right-xs-3").css("width", "62px");
    $(".k-grid-Config").removeClass("k-button k-button-icontext").addClass("btn btn-default i-gap-right-xs-3").css("width", "62px");
    $(".k-grid-GenPort").removeClass("k-button k-button-icontext").addClass("btn btn-default i-gap-right-xs-3").css("width", "80px");
    $(".k-grid-Restock").removeClass("k-button k-button-icontext").addClass("btn btn-default i-gap-right-xs-3").css("width", "72px");

    $(".k-grid-delete").removeClass("k-button k-button-icontext").addClass("btn btn-default i-gap-right-xs-3").css("width", "65px");
    $(".k-grid-edit").removeClass("k-button k-button-icontext").addClass("btn btn-default i-gap-right-xs-3").css("width", "62px");

    $(".k-edit").html("<i class='fa fa-pencil-square-o fa-lg'></i>&nbsp; ");
    $(".k-delete").html("<i class='fa fa-times-circle fa-lg'></i>&nbsp; ");

    $(".k-edit").removeClass();
    $(".k-delete").removeClass();
}