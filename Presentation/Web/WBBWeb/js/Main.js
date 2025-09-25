//http://stackoverflow.com/questions/1225102/jquery-event-to-trigger-action-when-a-div-is-made-visible
jQuery(function ($) {
    var _oldShow = $.fn.show;
    $.fn.show = function (speed, oldCallback) {
        return $(this).each(function () {
            var obj = $(this),
                newCallback = function () {
                    if ($.isFunction(oldCallback)) {
                        oldCallback.apply(obj);
                    }
                    obj.trigger('afterShow');
                };

            // you can trigger a before show if you want
            obj.trigger('beforeShow');

            // now use the old function to show the element passing the new callback
            _oldShow.apply(obj, [speed, newCallback]);
        });
    }
});

//http://jsfiddle.net/mofle/eZ4X3/
(function ($) {
    $.each(['show', 'hide'], function (i, val) {
        var _org = $.fn[val];
        $.fn[val] = function () {
            this.trigger(val);
            _org.apply(this, arguments);
        };
    });
})(jQuery);

//======Dropdown Select========

//    dropdown select ในกรณี ปรกติ
$(".select").select2({
    minimumResultsForSearch: Infinity

});

//    dropdown select ในกรณี required
$(".select-error").select2({
    minimumResultsForSearch: Infinity,
    theme: "classic"
});


//=======Style button Language Thai& Eng=====
//$(function () {
//    $('#btn_th').click(function () {
//        $(this).addClass('btn-success');
//        $('#btn_en').removeClass('btn-success').addClass('btn-default');
//    });
//    $('#btn_en').click(function () {
//        $(this).addClass('btn-success');
//        $('#btn_th').removeClass('btn-success').addClass('btn-default');
//    });

//});


//style button เลือกสถานที่  คอนโด หมู่บ้าน อื่นๆ
$(function () {
    $('#aisuser').hide();
    $('#new-cus').click(function () {
        $(this).addClass('btn-success');
        $('#ais-cus').removeClass('btn-success').addClass('btn-default');
        $('#NewUser').removeClass('hide');
        $('#aisuser').hide();
    });
    $('#ais-cus').click(function () {
        $(this).addClass('btn-success');
        $('#new-cus').removeClass('btn-success').addClass('btn-default');
        $('#NewUser').addClass('hide');
        $('#aisuser').show();
    });


});
$(document).ready(function () {

    $("#formVillage").hide();
    $("#formHouse").hide();


    $('#radio_1').click(function () {

        $("#formCondo").show();
        $("#formVillage").hide();
        $("#formHouse").hide();

    });
    $('#radio_2').click(function () {

        $("#formCondo").hide();
        $("#formVillage").show();
        $("#formHouse").hide();

    });
    $('#radio_3').click(function () {

        $("#formCondo").hide();
        $("#formVillage").hide();
        $("#formHouse").show();

    });

});

//Style  เลือกแพ็กเกจ
$("#play").change(function () {
    if ($(this).is(":checked")) {
        $('#playbox').addClass("add-rounded-green");
    } else {
        $('#playbox').removeClass("add-rounded-green");
    }
});

$("#fixed").change(function () {
    if ($(this).is(":checked")) {
        $('#fixedline').addClass("add-rounded-green");
    } else {
        $('#fixedline').removeClass("add-rounded-green");
    }
});

//Style Button สำหรับพนักงาน

$('#btn-officer').css("cursor", "pointer");
$('#btn-officer').click(function () {
    $('#forofficer').toggle();
    $('i.fa.fa-plus').toggleClass('fa-minus');

});

//Style Button สำหรับรารางจองคิวช่าง สำหรับ Responsive
$("#swipe").click(function () {
    $("#swipe > img").hide();

});