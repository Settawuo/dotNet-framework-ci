$(document).ready(function () {
    $('#btnResend').attr("disabled", true);

    var todayDate = new Date();
    $("#dtpDateFrom").data("kendoDatePicker").value();
    $("#dtpDateTo").data("kendoDatePicker").value();

});


function ValidationDateValue(model) {

    var chk = ValidationProcessing("dateselect");

    if (chk) //check blank value
    {
        //DateFrom
        if ($("#dtpDateFrom").val() != "") {
            if (!kendo.parseDate($("#dtpDateFrom").val(), "dd/MM/yyyy")) {

                $("#validatefor-" + "dtpDateFrom").html(ValidationIcon() + "This field is not correct format. : DD/MM/YYYY");
                chk = false;
            }
        }
        else {
            $("#validatefor-" + "dtpDateFrom").html(ValidationIcon() + "This field is required.");
            chk = false;
        }

        ///DateTo
        if ($("#dtpDateTo").val() != "") {
            if (!kendo.parseDate($("#dtpDateTo").val(), "dd/MM/yyyy")) {

                $("#validatefor-" + "dtpDateTo").html(ValidationIcon() + "This field is not correct format. : DD/MM/YYYY");
                chk = false;
            }

        }
        else {
            $("#validatefor-" + "dtpDateTo").html(ValidationIcon() + "This field is required.");
            chk = false;
        }

    }
    return chk;
}

var GetOrderErrorLogQuery = function () {
    P_DATE_FROM: "";
    P_DATE_TO: "";
    P_ID_CARD_NO: "";
    P_REQUEST_STATUS: "";
    P_PAGE_INDEX: "";
    P_PAGE_SIZE: "";

}

function defaultData() {

    var searchPara = new GetOrderErrorLogQuery();
    searchPara.P_DATE_FROM = $("#dtpDateFrom").val();
    searchPara.P_DATE_TO = $("#dtpDateTo").val();
    searchPara.P_ID_CARD_NO = $("#txtIDCardNo").val();
    searchPara.P_REQUEST_STATUS = $("#Status").val();
    searchPara.P_PAGE_INDEX = $("#gridOrderErrorLog").data("kendoGrid").dataSource._page;
    searchPara.P_PAGE_SIZE = $("#gridOrderErrorLog").data("kendoGrid").dataSource._pageSize;

    return searchPara;
}

function bindingValue() {
    var model = defaultData();
    return {
        dataS: JSON.stringify(model)
    };

}

function onGridDataBound() {
    Loading(0);

    $('#btnResend').attr("disabled", false);

    if (this.dataSource.view().length == 0) {
        //insert empty row
        var colspan = this.thead.find("th").length;
        var emptyRow = "<tr><td colspan='" + colspan + "'></td></tr>";
        this.tbody.html(emptyRow);

        //workarounds for IE lt 9
        //this.table.width(800);
        //$(".k-grid-content").width(2 * kendo.support.scrollbar());
        $(".k-grid-content").height(3 * kendo.support.scrollbar());

        $('#btnResend').attr("disabled", true);
    }
    else {
        $(".k-grid-content").height("auto");
    }
}

function _Search() {
    var model = defaultData();
    var chk = ValidationDateValue();

    var grid = $("#gridOrderErrorLog").data("kendoGrid");
    var scrollLeft = grid.content[0].scrollTop;
    grid.dataSource.data([]);
    grid.dataSource.pageSize(20);
    grid.content[0].scrollLeft = scrollLeft;
    grid.refresh();

    if (chk) {
        Loading();

        grid.dataSource.page(1);
    }


    if (kendo.parseDate($("#dtpDateFrom").val(), "dd/MM/yyyy")) {
        $("#validatefor-" + "dtpDateFrom").html("");
    }
    if (kendo.parseDate($("#dtpDateTo").val(), "dd/MM/yyyy")) {
        $("#validatefor-" + "dtpDateTo").html("");
    }

    }

function _Clear() {

    var todayDate = new Date();
    $("#dtpDateFrom").data("kendoDatePicker").value('');
    $("#dtpDateTo").data("kendoDatePicker").value('');

    //reset min, max.
    $("#dtpDateFrom").data("kendoDatePicker").max(new Date(2099, 11, 31, 0, 0, 0, 0));
    $("#dtpDateTo").data("kendoDatePicker").min(new Date(1900, 0, 1, 0, 0, 0, 0));

    $("#validatefor-dtpDateTo").html("");
    $("#validatefor-dtpDateFrom").html("");

    $('#btnResend').attr("disabled", true);


    var grid = $("#gridOrderErrorLog").data("kendoGrid");
    var scrollLeft = grid.content[0].scrollTop;
    grid.dataSource.data([]);
    grid.dataSource.pageSize(20);
    grid.content[0].scrollLeft = scrollLeft;
    grid.refresh();


    $("#Status").data("kendoDropDownList").select(0);
    $("#txtIDCardNo").val("");


}

function Change_dtpDateFrom() {

    var startPicker = $("#dtpDateFrom").data("kendoDatePicker").value();
    var endPicker = $("#dtpDateTo").data("kendoDatePicker").value();
    if (endPicker < startPicker
        && ($("#dtpDateFrom").data("kendoDatePicker").value() != ''
        && (kendo.parseDate($("#dtpDateFrom").val(), "dd/MM/yyyy")))) {
        $("#dtpDateTo").data("kendoDatePicker").value(startPicker);
    }

}

function Change_dtpDateTo() {

    var startPicker = $("#dtpDateFrom").data("kendoDatePicker").value();
    var endPicker = $("#dtpDateTo").data("kendoDatePicker").value();
    if ((endPicker < startPicker)
        && ($("#dtpDateTo").data("kendoDatePicker").value() != ''
        && (kendo.parseDate($("#dtpDateTo").val(), "dd/MM/yyyy")))
         ) {
        $("#dtpDateFrom").data("kendoDatePicker").value(endPicker);
    }

}

function onStatusChange() {
}

function EditOrder() {
      var myWindow = $("#editOrder");
    myWindow.kendoWindow({
        width: "1300",
        //height: "80%",
        title: "EditOrder",
        visible: false,
        actions: [
            "Pin",
            "Minimize",
            "Maximize",
            "Close"
        ],
    }).data("kendoWindow").center().open();
}