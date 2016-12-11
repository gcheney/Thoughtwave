$(document).ready(function() {

    // Navigation script to Show Header on Scroll-Up
    //primary navigation slide-in effect
    if ($(window).width() > 1170) {
        var headerHeight = $('.navbar-custom').height();
        $(window).on('scroll', {
                previousTop: 0
            }, function()   {
                var currentTop = $(window).scrollTop();
                //check if user is scrolling up
                if (currentTop < this.previousTop) {
                    //if scrolling up...
                    if (currentTop > 0 && $('.navbar-custom').hasClass('is-fixed')) {
                        $('.navbar-custom').addClass('is-visible');
                    } else {
                        $('.navbar-custom').removeClass('is-visible is-fixed');
                    }
                } else {
                    //if scrolling down...
                    $('.navbar-custom').removeClass('is-visible');
                    if (currentTop > headerHeight && !$('.navbar-custom').hasClass('is-fixed')) {
                        $('.navbar-custom').addClass('is-fixed');
                    }
                }
                this.previousTop = currentTop;
            }
        );
    }

    // navbar search form 
    $('a[href="#search"]').on('click', function(e) {
        e.preventDefault();
        $('#search').addClass('open');
        $('#search > form > input[type="search"]').focus();
    });
    
    $('#search, #search button.close').on('click keyup', function(e) {
        if (e.target == this || e.target.className == 'close' || e.keyCode == 27) {
            $(this).removeClass('open');
        }
    });

    // search panel form
    $('.search-panel .dropdown-menu').find('a').click(function(e) {
		e.preventDefault();
		var param = $(this).attr("href").replace("#","");
		var category = $(this).text();
		$('.search-panel span#search_category').text(category);
		$('.input-group #category').val(param);
	});


    // Handle logout form link
    $('#logoutLink').on('click', function(e) {
        e.preventDefault();
        $('#logoutForm').submit();
    });
    
});