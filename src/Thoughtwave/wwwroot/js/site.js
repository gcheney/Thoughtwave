/*
 * Custom scripts for Thoughtwave
 */

(function($, W, D) { 
    var JQUERY = {};

    JQUERY.UTIL = {
        initNavigationSlide: function() {
            if ($(W).width() > 1170) {
                var headerHeight = $('.navbar-custom').height();
                $(W).on('scroll', {
                        previousTop: 0
                    }, function()   {
                        var currentTop = $(W).scrollTop();
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
        },
        initNavbarSerach: function() {
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
        },
        initSearchPanelForm: function() {
            $('.search-panel .dropdown-menu').find('a').click(function(e) {
                e.preventDefault();
                var param = $(this).attr("href").replace("#","");
                var category = $(this).text();
                $('.search-panel span#search_category').text(category);
                $('.input-group #category').val(param);
            });
        },
        hanldeLogoutForm: function() {
            $('#logoutLink').on('click', function(e) {
                e.preventDefault();
                $('#logoutForm').submit();
            });
        },
        fadeOutFlashMessages: function() {
            function flashFadeout() {
                $('#flash-message').fadeTo('fast', 0.00, function() { 
                        $(this).slideUp('slow', function() { 
                            $(this).remove(); 
                        });
                });
            }

            // close flash message
            $('.close').on('click', function(e) {
                e.preventDefault();
                flashFadeout();
            });

            setTimeout(function() {
                flashFadeout();
            }, 10000);
        },
        enableAreYouSure: function() {
            $('form').areYouSure({
                'message': 'Your changes will not be saved!'
            });
        }
    }

    //when the dom has loaded setup form validation rules
    $(D).ready(function($) {
        JQUERY.UTIL.enableAreYouSure();
        JQUERY.UTIL.initNavigationSlide();
        JQUERY.UTIL.initNavbarSerach();
        JQUERY.UTIL.initSearchPanelForm();
        JQUERY.UTIL.hanldeLogoutForm();
        JQUERY.UTIL.fadeOutFlashMessages();
    });

})(jQuery, window, document);