; (function (tbSearch, undefined) {
    //private
    $(function () {
        $(window).on('scroll resize', tbSearch.debounce(function (e) {
            if ($('button#load-more').is(':in-viewport')) { // Check if visible on the screen
                $('button#load-more').trigger('click'); // Click button
            }
        }, 500));

        $(document).on("keyup", "#SearchTerm", tbSearch.debounce(function (e) {
            e.preventDefault();
            //close up the browse options and remove any selection
            $('#search-filters').collapse("hide");
            $('#search-form-wrapper').collapse("hide");
            $('input:radio[name=Browse]:checked').prop('checked', false);

            if ($(this).val() === '') {
                //show the browse options when search empty
                //$('#search-filters').collapse("show");

            }

            $('#BrowseTheme').html('');

            var form = $(e.target).closest('form');
            $(form).submit();

        }, 300));

        $("#search-filters .custom-radio [type=radio]").on("click", function (e) {
            $("#search-form-wrapper").collapse("hide"); // collapse the search menu when filteroption selected
            var target = e.target.id.replace(/_/g, " ");
            $('#BrowseTheme').html(target != "viewall" ? 'Filtered by ' + target : "");
            var form = $(e.target).closest('form');
            $(form).submit();
        });

        $('#search-toggler').on('click', function (e) {

            $('#search-form-wrapper').collapse('toggle');
            $('input:radio[name=Browse]:checked').prop('checked', false);
            $('#SearchTerm').val('');
            //$('#search-filters').collapse("toggle");
            $('#BrowseTheme').html('');

            var form = $('#menu-search-form');            
            $(form).submit();
        });

        $(document).on("click", 'a[id^="theme-btn-"]', function () {

            
            var theme = this.id.replace('theme-btn-', ''); 
            $('#'+ theme ).prop('checked', true);
            $('#SearchTerm').val('');
            $('#search-filters').collapse("hide");
            $('#BrowseTheme').html('Filtered by ' + theme.replace(/_/g, " "));

            var form = $('#menu-search-form');
            $(form).submit();
        });

    });
    //public 
    tbSearch.loadMoreSuccess = function (data) {
        // remove load-more form
        $(this).remove();
    };

    tbSearch.loadMoreBegin = function (data) {
        //disable button and inject the spinner
        $(this).find('#load-more').prop('disabled', true).html(
            "<span class='spinner-border spinner-border-sm' role='status' aria-hidden='true'></span> Loading More..."
        );
    };

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