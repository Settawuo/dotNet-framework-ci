function prepareSearch() {
var newinput = $("<div class='typeahead-container'>"+
				"<div class='typeahead-field'>"+
					"<span class='typeahead-query'>"+
						"<input id='globalsearch' name='q' class='textbox js-search-input' onKeypress='checkSize()' placeholder='Search...' autocomplete='off' type='text'>"+
					"</span>" +
				"</div>"+
				"</div>"+
				"<i id='removetext' class='search-remove-text' Onclick='removeText()'></i>");
					
$('#globalsearch').remove();
$('.top_search_input').append(newinput);

$('#globalsearch').on('click input',function() {
	$('.pushy.pushy-left > div').css('display','none');
	$('#ul-sub-menu > ul > li').removeClass('current');	
	
	if($('#globalsearch').val()) {
		$('#quicklink').addClass('hide');
		$('#quicklink').removeClass('show');
	} else {
		$('#quicklink').addClass('show');
		$('#quicklink').removeClass('hide');
	}	
	if($(window).width() > 500) {
		setTimeout(function(){
			$('.media-body .title-recommend').css('width',$('#topbar_search').width()-135);
			$('.recommend-desc-p').css('width',$('#topbar_search').width()-135);
		}, 200);
	}
});

$('#globalsearch').on('focusout',function() {
	setTimeout(function() {
	    $('#quicklink').addClass('hide');
		$('#quicklink').removeClass('show');
		
		if($(window).width() > 500) {
			$('.media-body .title-recommend').css('width','auto');
			$('.recommend-desc-p').css('width','auto');
		} else {
			$('.media-body .title-recommend').css('width','100px');
			$('.recommend-desc-p').css('width','100px');
		}
	}, 200);
	
});

$('#ul-sub-menu > ul > li').hover(function(){
	$('.typeahead-container').removeClass('result hint backdrop');
});

$('#globalsearch').on('keypress', function(event) {
	if(event.keyCode  == 13) {
		$('#topbar_search').submit();
	}
});

var domainName = window.location.host;
var max_matches = 5;
$( window ).resize(function() {
	if($( window ).height() <= 480) {
		max_matches = 3;
	} else {
		max_matches = 5;
	}
});


//var imagePath = 'http://110.49.202.141/yellowbox';
var imagePath = 'http://search.ais.co.th';
$('#globalsearch').typeahead({
	
	minLength: 1,
    maxItem: 10,
	delay: 250,
	dynamic: true,
	highlight: true,
    hint: true,
	href:'http://search.ais.co.th/search?q={{results}}',
	group: ["type", "{{group}}"],
	display: ["name","detail","tags"],
    //template: '<span>{{title}}</span>',
	template: 	'<a class="" target="_blank" href="{{link}}">'+
				'<div class="media-left">'+
				'	 <img class="media-object image-suggest-recom {{cssClass}}" src="'+imagePath+'{{imgSrc}}" alt="{{originalImageName}}">'+
				'</div>'+
				'<div class="media-body">'+
				'	<span class="media-heading media-heading-label title-{{cssClass}}">{{name}}</span>'+
				'	<p class="recommend-desc-p {{cssClass}}">{{detail}}</p>'+
				'</div>'+
				'</a>',
//    emptyTemplate: "No results found for <b>{{query}}<b>",
    source: {
		'Common results': {
			url: [{ 
				
				url: 'http://search.ais.co.th/api/gsa/suggest?',
				//url: 'http://localhost:9000/yellowbox/api/gsa/suggest?',				
				//url: 'http://110.49.202.141/yellowbox/api/gsa/suggest?',
				data:{
					token: function () { return $('#globalsearch').val(); },
					max_matches: function () { return max_matches; }
				},
				type: 'GET',
				callback: {
					done: function (data) {
//							console.log("data.results===== " ,data.results);
							var results = data.results;
							return results;
					}
				}
			}]
		}
		
    },callback: {
		    onClickAfter: function (node, a, item, event) {
	            // href key gets added inside item from options.href configuration
		    	if(item.linkAction === "goTo") {
					window.open(item.link, '_self');
				} else {
					window.location = 'http://search.ais.co.th/search?q='+item.name;
					//window.location = 'http://localhost:9000/yellowbox/search?q='+item.name;
				}
	        }
		}
});
//var postUrl = 'http://110.49.202.141/yellowbox/api/cms/getquicklink';		
//var postUrl = 'http://localhost:9000/yellowbox/api/cms/getquicklink';		
var postUrl = 'http://search.ais.co.th/api/cms/getquicklink';
$.post( postUrl, function( data ) {
$('.typeahead-container').append("<div id='quicklink' class='quicklink hide'><span>Quick Links</span><ul></ul></div>");
	var quicklinkdata = "";
	if(data.results) {
		for(var i=0 ; i< data.results.length ; i ++) {
			if(data.results[i].name != 'hr') {
				quicklinkdata += "<li>"+
							 "<a target=\"_self\" href=\"#\" onclick=\"quicklinkClick('"+ data.results[i].link + "' ,'" + data.results[i].name+"');return false;\">"+
							 "<div class='media-left'><img class=\"media-object image-suggest-recom "+ data.results[i].cssClass +"\" src=\"" + imagePath + data.results[i].imgSrc +"\" alt=\""+ data.results[i].originalImageName +"\"></div>"+
							 "<div class='media-body'><span class=\"media-heading media-heading-label title-"+data.results[i].cssClass+" \">"+ data.results[i].name +"</span><p class='recommend-desc-p "+ data.results[i].cssClass +"'>"+ data.results[i].detail +"</p></div>"+
							 "</a></li>";
			 } else {
				quicklinkdata += "<li><hr></li>";
			 }
		}
		$('#quicklink > ul').append(quicklinkdata);
	}

});


} // end of function


function quicklinkClick(link, name) {
	if(link) {
		window.open(link, '_self');
	} else {
		window.location = 'http://search.ais.co.th/search?q='+name;
		//window.location = 'http://localhost:9000/yellowbox/search?q='+name;
	}
}

function removeText() {
	$('#globalsearch').val("");
	checkSize();
}

function checkSize() {
	 setTimeout(function() {
		 if($('#globalsearch').val().length > 0) {
			 $('#removetext').css('visibility','visible');
		 } else {
			 $('#removetext').css('visibility','hidden');
		 }
	 },170);
}

