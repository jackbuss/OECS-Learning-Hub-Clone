﻿; (function (tbSearch, undefined) {
    //private
    $(function () {
        $(document).on("click", "#search-button", function (e) {
            $("div#search-results").empty();
            e.preventDefault();
            var form = $(this).closest('form');
            $(form).submit();
        });


        //$(document).on("click", "#top-search-button", function (e) { // Clear search results div when form submit to load results
        //    $("div#search-results").empty();
        //});

        $(document).on("click", "#next-button", function (e) { // "Load More" press
            e.preventDefault();
            var form = $(this).closest('form');
            $(form).submit();
            $(this).closest("ul").remove(); // Remove load more after so avoidng page jumping
        });

        //window.onscroll = function (e) { // Scroll to bottom
        //    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
        //        $("button#next-button").click(); // Click "Load More" button
        //    }
        //};

        $(window).on('scroll resize', tbSearch.debounce(function (e) {
            if ($('button#next-button').is(':in-viewport')) { // Check if visible on the screen
                $('button#next-button').trigger('click'); // Click button
            }
        }, 500));

        $(document).on("keyup", "#SearchTerm", tbSearch.debounce(function (e) {
            $("div#search-results").empty();
            e.preventDefault();
            var form = $(e.target).closest('form');
            $(form).submit();
        }, 300));

    });
    //public 
    tbSearch.debounce = function (func, wait, immediate) {
        var timeout;
        return function () {
            var context = this, args = arguments;
            var later = function () {
                timeout = null;
                if (!immediate) func.apply(context, args);
            };
            var callNow = immediate && !timeout;
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
            if (callNow) func.apply(context, args);
        };
    };

})(window.tbSearch = window.tbSearch || {});