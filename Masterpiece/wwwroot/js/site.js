﻿$(document).ready(function () {
    // Function to animate the horizontal selector
    function animateSelector(element) {
        var activeWidthNewAnimHeight = element.innerHeight();
        var activeWidthNewAnimWidth = element.innerWidth();
        var itemPosNewAnimTop = element.position();
        var itemPosNewAnimLeft = element.position();
        $(".hori-selector").css({
            "top": itemPosNewAnimTop.top + "px",
            "left": itemPosNewAnimLeft.left + "px",
            "height": activeWidthNewAnimHeight + "px",
            "width": activeWidthNewAnimWidth + "px"
        });
    }

    // Initial animation setup for the active link
    var activeItem = $('#navbarSupportedContent .nav-item.active .nav-link');
    if (activeItem.length) animateSelector(activeItem);

    // Add click event listener for nav links
    $('#navbarSupportedContent .nav-link').on('click', function (e) {
        e.preventDefault(); // Prevent immediate navigation
        var target = $(this);
        var targetUrl = target.attr('href');

        // Handle animation
        $('#navbarSupportedContent .nav-item').removeClass('active');
        target.parent().addClass('active');
        animateSelector(target);

        // Delay navigation slightly to allow the animation to play
        setTimeout(function () {
            window.location.href = targetUrl;
        }, 500); // Adjust delay as needed for smoothness
    });
});



//////////////////////////////////////imgSliderattop///////////////////////////////////////////

$(document).ready(function () {
   


var $slider = $('.slideshow .slider'),
    maxItems = $('.item', $slider).length,
    dragging = false,
    tracking,
    rightTracking;

$sliderRight = $('.slideshow').clone().addClass('slideshow-right').appendTo($('.split-slideshow'));

rightItems = $('.item', $sliderRight).toArray();
reverseItems = rightItems.reverse();
$('.slider', $sliderRight).html('');
for (i = 0; i < maxItems; i++) {
    $(reverseItems[i]).appendTo($('.slider', $sliderRight));
}

$slider.addClass('slideshow-left');
$('.slideshow-left').slick({
    vertical: true,
    verticalSwiping: true,
    arrows: false,
    infinite: true,
    dots: true,
    speed: 1000,
    cssEase: 'cubic-bezier(0.7, 0, 0.3, 1)'
}).on('beforeChange', function (event, slick, currentSlide, nextSlide) {

    if (currentSlide > nextSlide && nextSlide == 0 && currentSlide == maxItems - 1) {
        $('.slideshow-right .slider').slick('slickGoTo', -1);
        $('.slideshow-text').slick('slickGoTo', maxItems);
    } else if (currentSlide < nextSlide && currentSlide == 0 && nextSlide == maxItems - 1) {
        $('.slideshow-right .slider').slick('slickGoTo', maxItems);
        $('.slideshow-text').slick('slickGoTo', -1);
    } else {
        $('.slideshow-right .slider').slick('slickGoTo', maxItems - 1 - nextSlide);
        $('.slideshow-text').slick('slickGoTo', nextSlide);
    }
}).on("mousewheel", function (event) {
    event.preventDefault();
    if (event.deltaX > 0 || event.deltaY < 0) {
        $(this).slick('slickNext');
    } else if (event.deltaX < 0 || event.deltaY > 0) {
        $(this).slick('slickPrev');
    };
}).on('mousedown touchstart', function () {
    dragging = true;
    tracking = $('.slick-track', $slider).css('transform');
    tracking = parseInt(tracking.split(',')[5]);
    rightTracking = $('.slideshow-right .slick-track').css('transform');
    rightTracking = parseInt(rightTracking.split(',')[5]);
}).on('mousemove touchmove', function () {
    if (dragging) {
        newTracking = $('.slideshow-left .slick-track').css('transform');
        newTracking = parseInt(newTracking.split(',')[5]);
        diffTracking = newTracking - tracking;
        $('.slideshow-right .slick-track').css({ 'transform': 'matrix(1, 0, 0, 1, 0, ' + (rightTracking - diffTracking) + ')' });
    }
}).on('mouseleave touchend mouseup', function () {
    dragging = false;
});

$('.slideshow-right .slider').slick({
    swipe: false,
    vertical: true,
    arrows: false,
    infinite: true,
    speed: 950,
    cssEase: 'cubic-bezier(0.7, 0, 0.3, 1)',
    initialSlide: maxItems - 1
});
$('.slideshow-text').slick({
    swipe: false,
    vertical: true,
    arrows: false,
    infinite: true,
    speed: 900,
    cssEase: 'cubic-bezier(0.7, 0, 0.3, 1)'
});

});

    function toggleNavbar() {
    const nav = document.getElementById("navbarSupportedContent");
    nav.classList.toggle("show");
}

