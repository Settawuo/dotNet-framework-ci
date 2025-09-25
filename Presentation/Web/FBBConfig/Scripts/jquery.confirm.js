/**
 * jquery.confirm
 * @author My C-Labs
 * @author Matthieu Napoli <matthieu@mnapoli.fr>
 * @url https://github.com/myclabs/jquery.confirm
 */
(function($) {
	/**
	 * Confirm a link or a button
	 * @param options {text, confirm, cancel, confirmButton, cancelButton, post}
	 */
	$.fn.confirm = function(options) {
		if (typeof options === 'undefined') {
			options = {};
		}

		options.button = $(this);

		this.click(function(e) {
			e.preventDefault();

			$.confirm(options);
		});

		return this;
	};

	/**
	 * Show a confirmation dialog
	 * @param options {text, confirm, cancel, confirmButton, cancelButton, post}
	 */
	$.confirm = function(options) {
		// Options
		if (typeof options === 'undefined') {
			options = {};
		}
		if (typeof options.confirm === 'undefined') {
		    options.confirm = function () {
		        setTimeout(function () {
		            $('#divMessageBox').kendoWindow('destroy');
		        }, 200);
		    };
		}
		if (typeof options.cancel === 'undefined') {
		    options.cancel = function () {
		        setTimeout(function () {
		            $('#divMessageBox').kendoWindow('destroy');
		        }, 200);
		    };
		}
		if (typeof options.text === 'undefined') {
			options.text = "Are you sure?";
		}
		if (typeof options.confirmButton === 'undefined') {
			options.confirmButton = "Yes";
		}
		if (typeof options.cancelButton === 'undefined') {
			options.cancelButton = "Cancel";
		}
		// Show the modal
		var modalHTML = '<div class="popupMsgContent" id="divMessageBox">'
            + '<h1>Confirm ?</h1>'
			+ '<p>' + options.text + '</p>'
	        + '<input type="submit" id="btnMessageSubmit" value="' + options.confirmButton + '" class="confirm-flat-button" onclick="return ' + options.confirm + '">'
	        + '<input type="submit" id="btnMessageClose" value="' + options.cancelButton + '" class="cancel-flat-button">';
		+ '</div>';
		var modal = $(modalHTML);
		modal.find("#btnMessageSubmit").click(function (e) {
		    options.confirm();
		});
		modal.find("#btnMessageClose").click(function (e) {
		    options.cancel();
		});
		$(document.body).append(modal);
	    $('#divMessageBox').kendoWindow({
	        modal: true,
	        resizable: false,
	        width: 400,
	        close: function () {
	            setTimeout(function () {
	                $('#divMessageBox').kendoWindow('destroy');
	            }, 200);
	        }
	    }).data('kendoWindow').center();
	}

	$.alert = function (text, msgType) {
	    // Options	   
	    var options = {};
	    if (msgType === 'undefined') {
	        options.div_class = "popMsg-Info";
	        options.button_class = "green-flat-button";
	        options.title = "Information";
	    }
	    else
	    {
	        if (msgType == 'I') {
	            options.div_class = "popMsg-Info";
	            options.button_class = "green-flat-button";
	            options.title = "Information";
	        }
	        else if (msgType == 'E') {
	            options.div_class = "popMsg-Error";
	            options.button_class = "red-flat-button";
	            options.title = "Error!";
	        }
	        else if (msgType == 'W') {
	            options.div_class = "popMsg-Warning";
	            options.button_class = "orange-flat-button";
	            options.title = "Warning!";
	        }
	        else {
	            options.div_class = "popMsg-Info";
	            options.button_class = "green-flat-button";
	            options.title = "Information";
	        }
	    }

	    var modalHTML = '<div class="popupMsgContent" id="divMessageBox">'
            + '<h1>'+options.title+'</h1>'
			+ '<p>' + text + '</p>'
	        + '<input type="submit" id="btnMessageClose" value="OK" class="'+options.button_class+'">'
	        + '</div>';

	    $(document.body).append(modalHTML);

	    $('#divMessageBox').kendoWindow({
	        modal: true,
	        resizable: false,
	        width: 400,
	        close: function() {
	                setTimeout(function() {
	                    $('#divMessageBox').kendoWindow('destroy');
	                }, 200);
	        }
	    }).data('kendoWindow').center();

	    $('#divMessageBox').parent().addClass(options.div_class);

	    $('#btnMessageClose').click(function () {
	        setTimeout(function () {
	            $('#divMessageBox').kendoWindow('destroy');
	        }, 200);
	    });
	}
	$.info = function (text) {
	    $.alert(text, 'I');
	}

	$.warning = function (text) {
	    $.alert(text, 'W');
	}

	$.error = function (text) {
	    $.alert(text, 'E');
	}

})(jQuery);
