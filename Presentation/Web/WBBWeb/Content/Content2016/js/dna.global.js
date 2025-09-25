

function setButton(ID){
	var Target = document.getElementById(ID);
	var Set = true;
	try{ if (Device == 'Mobile') {Set = false;}}catch(e){}
	if(Set){
		Target.onmouseover=function () {
			TweenMax.fromTo(Target, 0.6, {scale:1.5}, {scale:1, ease:Elastic.easeOut}) ;
		};
   		Target.onmouseout=function () {
			TweenMax.fromTo(Target, 0.8, {scale:0.8}, {scale:1, ease:Elastic.easeOut}) ;
		};
	}
}

try{root}catch(e){var menuArr=[];var newContent='home'}// ทดสอบ Run ด้วยไฟล์ Interface


function setMenuButton(ID, Page, URL){
	var Target = document.getElementById(ID);
	var Set = true;
	menuArr.push([ID,Page]);
	setupMenu();
	if(URL){
		Target.onclick=function () {
			changeContent(URL);
		}
	}
	
	try{ if (Device == 'Mobile') {
		Set = false;
	}}catch(e){}
	
	if(Set){
		Target.onmouseover=function () {
			document.getElementById(ID+'_on').style.display = 'block';
			document.getElementById(ID+'_off').style.display = 'none';
			TweenMax.fromTo(Target, 0.6, {y:15}, {y:0, ease:Elastic.easeOut}) ;
		}
		Target.onmouseout=function () {
			if(newContent != Page){
				document.getElementById(ID+'_on').style.display = 'none';
				document.getElementById(ID+'_off').style.display = 'block';
				TweenMax.fromTo(Target, 0.8, {y:-15}, {y:0, ease:Elastic.easeOut}) ;
			}
		}
	}
}

function setupMenu(LockID){
	//console.log('setupMenu(): newContent:'+newContent+' / allMenu:'+menuArr.length);
	for(var i = 0; i<menuArr.length; i++){
		if(menuArr[i][1] == newContent  && !LockID){
			document.getElementById(menuArr[i][0]+'_on').style.display = 'block';
			document.getElementById(menuArr[i][0]+'_off').style.display = 'none';			
		}else{
			document.getElementById(menuArr[i][0]+'_on').style.display = 'none';
			document.getElementById(menuArr[i][0]+'_off').style.display = 'block';
		}
	}
	
	
	if(LockID){
		document.getElementById(LockID+'_on').style.display = 'block';
		document.getElementById(LockID+'_off').style.display = 'none';		
	}
}


function drop() {
   if (!$.browser.opera) {
      // select element styling
      $('select.select').each(function () {
         var title = $(this).attr('title');
         if ($('option:selected', this).val() != '') title = $('option:selected', this).text();
         $(this)
            .css({
               'z-index': 10,
               'opacity': 0,
               '-khtml-appearance': 'none'
            })
            .after('<span class="select">' + title + '</span>')
            .change(function () {
               val = $('option:selected', this).text();
               $(this).next().text(val);
            })
      });
   };
}

function loadjscssfile(filename, filetype, onload) {
    //if filename is a external JavaScript file
    if (filetype == "js") { 
        var fileref = document.createElement('script');
        fileref.type = "text/javascript";
        fileref.onload = onload;
        fileref.src = filename;
        document.getElementsByTagName("head")[0].appendChild(fileref);
        return;
    }
    //if filename is an external CSS file
    if (filetype == "css") { 
        var fileref = document.createElement("link");
        fileref.rel = "stylesheet";
        fileref.type = "text/css";
        fileref.onload = onload;
        fileref.href = filename;
        document.getElementsByTagName("head")[0].appendChild(fileref);
        return;
    }
}

function loadJS(filename, onload){
	if (filename!="undefined"){
		if(onload){
			loadjscssfile(filename,"js", onload);
		}else{
			loadjscssfile(filename,"js", function() { trace("loadCSS("+filename+"): loaded"); });
		}
	}
}

function loadCSS(filename, onload){
	if (filename!="undefined"){
		if(onload){
			loadjscssfile(filename,"css", onload);
		}else{
			loadjscssfile(filename,"css", function() { trace("loadCSS("+filename+"): loaded"); });
		}
	}
}

function MM_preloadImages() { //v3.0
  var d=document; if(d.images){ if(!d.MM_p) d.MM_p=new Array();
    var i,j=d.MM_p.length,a=MM_preloadImages.arguments; for(i=0; i<a.length; i++)
    if (a[i].indexOf("#")!=0){ d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];}}
}

function MM_swapImgRestore() { //v3.0
  var i,x,a=document.MM_sr; for(i=0;a&&i<a.length&&(x=a[i])&&x.oSrc;i++) x.src=x.oSrc;
}

function MM_findObj(n, d) { //v4.01
  var p,i,x;  if(!d) d=document; if((p=n.indexOf("?"))>0&&parent.frames.length) {
    d=parent.frames[n.substring(p+1)].document; n=n.substring(0,p);}
  if(!(x=d[n])&&d.all) x=d.all[n]; for (i=0;!x&&i<d.forms.length;i++) x=d.forms[i][n];
  for(i=0;!x&&d.layers&&i<d.layers.length;i++) x=MM_findObj(n,d.layers[i].document);
  if(!x && d.getElementById) x=d.getElementById(n); return x;
}

function MM_swapImage() { //v3.0
  var i,j=0,x,a=MM_swapImage.arguments; document.MM_sr=new Array; for(i=0;i<(a.length-2);i+=3)
   if ((x=MM_findObj(a[i]))!=null){document.MM_sr[j++]=x; if(!x.oSrc) x.oSrc=x.src; x.src=a[i+2];}
}
function load_interface(){
	$( "#load_toppc" ).load( "interface.html #toppc" );
	//$( "#load_menumobile" ).delay( 200 ).load( "interface.html #menu_mobile" );
	$( "#load_slidemenu" ).delay( 400 ).load( "interface.html #slide_interface" );
	$( "#load_footer" ).delay( 600 ).load( "interface.html #footer" );
	}
function pakage_off(){
		$('#pakage_sideoff').slideUp();
		$('#pakage_sideon').slideDown();
	}
function pakage_on(){
		$('#pakage_sideon').slideUp();
		$('#pakage_sideoff').slideDown();
	}	

function menumobile(){
var  ContentY = 1;
$(window).scroll(function () {
	 if ($(window).scrollTop() >= 80){
		 
		 $("#menu_mobile").addClass("menufix");

    }else{
		 $("#menu_mobile").removeClass("menufix");
		}
});



} 
function showid(id){
	$("#"+id).show();
	}
function hideid(id){
	$("#"+id).hide();
	}
	
function headmenu_click(){
		$("#headmenu").hide();
		$("#headmenufull").show();
		$("#submobile").show();
	}
function headmenufull_click(){	
		$("#headmenufull").hide();
		$("#headmenu").show();
		$("#submobile").hide();
	
	}

function clicktab(id){
	$( ".xtratalkrow1" ).removeClass( "curent" );
	$( "#"+id ).find( ".xtratalkrow1" ).addClass( "curent" );
}
function easicheck(id){
$('#'+id+' :checkbox').click(function() {
    var $this = $(this);
    // $this will contain a reference to the checkbox   
    if ($this.is(':checked')) {
       $( "#"+id ).addClass( "bg_dark" );
    } else {
       $( "#"+id ).removeClass( "bg_dark" );
    }
});
}