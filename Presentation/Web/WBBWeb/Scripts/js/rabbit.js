function showDialog(x, y) {
    $("#ic-pop").removeClass();
    $("#ic-pop i").removeClass();

    if (x == "succ") {
        $("#ic-pop").addClass("i-icon-msg i-green");
        $("#ic-pop i").addClass("fa fa-check");
    }
    else if (x == "dupp") {
        $("#ic-pop").addClass("i-icon-msg i-green");
        $("#ic-pop i").addClass("fa fa-check-square-o");
    }
    else if (x == "err") {
        $("#ic-pop").addClass("i-icon-msg i-red");
        $("#ic-pop i").addClass("fa fa-times");
    }
    else {
        $("#ic-pop").addClass("i-icon-msg i-red");
        $("#ic-pop i").addClass("fa fa-warning");
    }

    $("#lb-popup").text(y);
    $("#myModal").modal();
}

function Loading(x) {
    if (x == 0) {
        $("#loading").modal("hide");
    }
    else {
        $("#loading").modal();
    }
}

function clearPageScrollUp(idClass) {
    $(idClass + " a[href=#]").removeAttr("href");
}