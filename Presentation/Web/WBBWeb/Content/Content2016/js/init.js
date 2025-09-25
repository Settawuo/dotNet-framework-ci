$(window).load(function() {
  "use strict";
  $("#preloader").addClass("hidden");
});

$(document).ready( function () {
  "use strict";
  
    // variable
    var screen_width = $(window).width();
    $("textarea").autosize();

    var nav_width = $("body header nav ul");
    nav_width.css("width", screen_width);

    //on resize run function rsizeItems()
    var tOut = false;
    var milSec = 500;
    $(window).resize(function(){
     if(tOut !== false)
        clearTimeout(tOut);
     tOut = setTimeout(rsizeItems, milSec);
    });
    function rsizeItems()
    {
      //scripts for resolutions smaller than 768px
      var screen_dinamic_width1 = $(window).width();
      nav_width.css("width", screen_dinamic_width1);
    }

    /*//variable for scroll page
    var top_ofset = 0;
    //check if firefox
    if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
        $("header li a, #subheader .scroll_down, #to_the_top").on("click",function(e){
          e.preventDefault();
          $("html, body").animate({
            scrollTop: $( $(this).attr("href") ).offset().top - top_ofset
          }, 1000);
        });        
    } 
    //detect touch device
    else if ("ontouchstart" in window || navigator.msMaxTouchPoints)
      {
        //scroll page on click (header links, subheader link, to_the_top link)
          $("header li a, #subheader .scroll_down, #to_the_top").on("click",function(){
            $("html, body").animate({
              scrollTop: $( $(this).attr("href") ).offset().top - top_ofset
            }, 1000);
          });
      }
      else {
        //scroll page on click (header links, subheader link, to_the_top link)
          $("header li a, #subheader .scroll_down, #to_the_top").on("click",function(e){
            //e.preventDefault();
            //e.isDefaultPrevent();
            $("html, body").animate({
              scrollTop: $( $(this).attr("href") ).offset().top - top_ofset
            }, 1000);
            
          });
      }
    //dropdown menu show/hide
    $("header nav span").on("click",function(){
      $(this).siblings().toggleClass("active");
    });
    $("header nav a").on("click",function(){
      $(this).parent().parent().removeClass("active");
    });*/


//variable for scroll page
    var top_ofset = 0;
    // for smooth scroll page we used this plugin https://github.com/kswedberg/jquery-smooth-scroll
    $('header li a, #subheader .scroll_down, #to_the_top,a').smoothScroll({
      offset: - top_ofset,
      // one of 'top' or 'left'
      direction: 'top',
      // only use if you want to override default behavior
      scrollTarget: null,
      // fn(opts) function to be called before scrolling occurs.
      // `this` is the element(s) being scrolled
      beforeScroll: function() {},
      // fn(opts) function to be called after scrolling occurs.
      // `this` is the triggering element
      afterScroll: function() {},
      easing: 'swing',
      speed: 800,
      // coefficient for "auto" speed
      autoCoefficent: 2,
      // $.fn.smoothScroll only: whether to prevent the default click action
      preventDefault: true      
    });
    
    //dropdown menu show/hide
    $("header nav span").on("click",function(){
      $(this).siblings().toggleClass("active");
    });
    $("header nav a").on("click",function(){
      $(this).parent().parent().removeClass("active");
    });






    function sliders() {
     //Screenshots slider
     //more options you can find at http://www.owlgraphic.com/owlcarousel/#customizing
      var owl1 = $("#phone_slider");
      owl1.owlCarousel({
        items : 3, //3 items above 1171px browser width
        itemsMobile : [1170,1], //1 item between 1170 and 0
        slideSpeed: 200,
        autoPlay: false
      });
       
      // Custom Navigation Events
      $("#screenshots .slider_navigation .next").click(function(){
        owl1.trigger("owl.next");
      });
      $("#screenshots .slider_navigation .prev").click(function(){
        owl1.trigger("owl.prev");
      });


     //Team slider
     //more options you can find at http://www.owlgraphic.com/owlcarousel/#customizing
      var owl2 = $("#team_slider");
      owl2.owlCarousel({
        items : 5, //5 items above 1701px browser width
        itemsDesktop : [1700,4], //4 items between 1700px and 1201px
        itemsDesktopSmall : [1200,3], //3 items betweem 1200px and 901px
        itemsTablet: [900,2], //2 items between 900 and 601
        itemsMobile : [600,1], //1 item between 600 and 0
        slideSpeed: 500,
        autoPlay: false
      });
       
      // Custom Navigation Events
      $("#team .slider_navigation .next").click(function(){
        owl2.trigger("owl.next");
      });
      $("#team .slider_navigation .prev").click(function(){
        owl2.trigger("owl.prev");
      });
    }

    //function for start animation
    function start_animation() {
      if (screen_width >= 768) {
        var s = skrollr.init({
          suffixes: ["bottom-visible"],
          smoothScrolling: true ,
          forceHeight: true ,


        });
      }
      $("body").addClass("active").delay(0).queue(function(){
        $("body.active #phone .clean_subhead").addClass("fadeIn");
      });
      $(".content, #subheader, footer").fadeIn("fast");
      sliders();
    }

    //start animation function
      start_animation();

    //if device is tablet or mobile
    if (screen_width < 768) {
        start_animation();
    }

    // //if device desktop
    // else if (screen_width >= 768) {  

    // }
    //add class and remove class to elements when page scroll
    $(window).scroll(function() {
        if ($(window).scrollTop()<520) {
          
        }
        if ($(window).scrollTop()>520) {
          $("body.active #section").addClass("active");
          $("#phone .clean_subhead").addClass("fast_transition");

        }
    });

    //add class active to screenshots  when screenshots is visible
    $("#phone_slider").bind("inview", function(event, isInView, visiblePartX, visiblePartY) {
      if (isInView) {
        $("body.active #screenshots").addClass("active");
      }
      else {
        // $("body.active #screenshots").removeClass("active");
      }
    });

    //add class active to form when become visible, and remove class hidden from .phone_x2 .p2
    $(".support_form").bind("inview", function(event, isInView, visiblePartX, visiblePartY) {
      if (isInView) {
        $(this).addClass("active");
        $(".phone_x2 .p2").removeClass("hidden");
      }
    });

    //add class active to .plans li when become visible
    $(".plans li").bind("inview", function(event, isInView, visiblePartX, visiblePartY) {
      if (isInView) {
        $(this).addClass("active");
      }
      else {
      }
    });

    //add class active to #wait h2 and span when become visible
    $("#wait *").bind("inview", function(event, isInView, visiblePartX, visiblePartY) {
      if (isInView) {
        $(this).addClass("active");
      }
      else {
      }
    });

    //add class active to #work li when become visible
    $("#work li").bind("inview", function(event, isInView, visiblePartX, visiblePartY) {
      if (isInView) {
        $(this).addClass("active");
      }
      else {
      }
    });


    //clone #phone children elements to .phone_clone
    $("#phone").children().clone().appendTo(".phone_clone");


    //click on team button to show details text
    $("#team button.view").click(function(e){
      e.preventDefault();
      $(this).addClass("deactive").parent().siblings().removeClass("active");
      $(this).next().addClass("active").parent().parent().addClass("active seen");
      $("#team .seen").click(function(){
        $(this).addClass("active").siblings().removeClass("active");
      });
    });


    //generate text from .feature_content to .feature
    $(".feature_content > div").each(function(){
      var current_class = "." + $(this).attr("class");
      var element_heading = $(this).find("h3").text();
      var element_text = $(this).find(".text p:first-of-type").text();

      //slice nummber of words
      var words_count = 10 + 1;

      //this function return words from element_text
      function getWords(element_text) {
          return element_text.split(/\s+/).slice(1, words_count).join(" ");
      }
      $("<h3>" + element_heading + "</h3>").insertBefore(".feature_wrap " + current_class + ".text_wrap .text");
      $("<p>" + getWords(element_text) + " ...</p>").insertBefore(".feature_wrap " + current_class + " .text_wrap .text a");
    });


    //lightbox open on click features links
     $(".feature a.img_wrap, .feature a.read_more").prettyPhoto();

  
    //phone clean img bgr
    var subheader_height = $("#subheader").height();
    var bg = $("#subheader .sub_inner").css("background-image");
    $("#phone .clean_subhead .img_wrap, .phone_clone .clean_subhead .img_wrap").css({
      "width": screen_width,
      "height": subheader_height,
      "margin-left": - (screen_width/2) + 2,
      
    });



    //scroll to the top icon
    $(window).scroll(function(){
        if ($(this).scrollTop() > 100) {
            $("#to_the_top").fadeIn();
        } else {
            $("#to_the_top").fadeOut();
        }
    });
    $("#to_the_top").click(function(){
        $("html, body").animate({ scrollTop: 0 }, 600);
        $(this).fadeOut(500);
        return false;
    });
 
    //lazy load vimeo video when video is visible
    $(".lazy_load_video.vimeo").bind("inview", function(event, isInView, visiblePartX, visiblePartY) {
      if (isInView) {
        var id = $(this).data("vimeo-id"),
        iframe = $("<iframe frameborder='0' webkitAllowFullScreen mozallowfullscreen allowFullScreen></iframe>");
        iframe.attr("src", "http://player.vimeo.com/video/" + id);
        $(this).replaceWith(iframe);
      }
      else {
        
      }
    });

    //lazy load youtube video when video is visible

    // $(".lazy_load_video.youtube").bind("inview", function(event, isInView, visiblePartX, visiblePartY) {
    //   if (isInView) {
    //     var id = $(this).data("youtube-id"),
    //     iframe = $("<iframe width="560" height="315" frameborder="0" allowfullscreen></iframe>");
    //     iframe.attr("src", "http://www.youtube.com/embed/" + id + "?showinfo=0");
    //     $(this).replaceWith(iframe);
    //   }
    //   else {
        
    //   }
    // }); 


// PLACEHOLDER
    $("[placeholder]").focus(function() {
      var input = $(this);
      if (input.val() == input.attr("placeholder")) {
        input.val("");
        input.removeClass("placeholder");
      }
    }).blur(function() {
      var input = $(this);
      if (input.val() === "" || input.val() == input.attr("placeholder")) {
        input.addClass("placeholder");
        input.val(input.attr("placeholder"));
      }
    }).blur().parents("form").submit(function() {
      $(this).find("[placeholder]").each(function() {
        var input = $(this);
        if (input.val() == input.attr("placeholder")) {
          input.val("");
        }
      });
    });

});


