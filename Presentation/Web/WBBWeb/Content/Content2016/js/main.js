// JavaScript Document

$(document).ready(function(){
  	$('input').iCheck({
   		 checkboxClass: 'icheckbox_flat-green', radioClass: 'iradio_flat-green'

	});



$('#pack1').css("cursor","pointer");
     
	 $("#pack1").click(function(){
		 
   			 $(".idetail1").toggleClass("idetailup");

        	 	$("#showpack1").toggle();

	 });
	 
$('#pack2').css("cursor","pointer");	

	  $("#pack2").click(function(){
		  $(".idetail2").toggleClass("idetailup");
		  
        	$("#showpack2").toggle();
 
    	});
		
$('#pack3').css("cursor","pointer");	

	  $("#pack3").click(function(){
		  $(".idetail3").toggleClass("idetailup");
		  
        	$("#showpack3").toggle();
 
    	});
});