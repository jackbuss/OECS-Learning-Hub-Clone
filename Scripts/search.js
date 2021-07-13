
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
            collapseAllAndShow(null);
            var target = e.target.id.replace(/_/g, " ");
            $('#BrowseTheme').html(target != "viewall" ? '' + target : "");
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
            $('#' + theme).prop('checked', true);
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


// CUSTOM

var collapses = ["#economy-collapse", "#environment-collapse", "#social-collapse", "#resources-collapse"];
var buttons = [".economy-button-label", ".environment-button-label", ".social-button-label", ".resources-button-label"];


function collapseAllAndShow(element) {
    for (i = 0; i < collapses.length; i++) {
        if (collapses[i] != element) {
            $(collapses[i]).collapse('hide')
        } else {
            $(collapses[i]).collapse('show');
        }
    }
    //$(".green-collapse").collapse('hide');
    if (element != null) {
        $(element).collapse('show');
    }
}

function changeColour(element) {
    for (i = 0; i < buttons.length; i++) {
        $(buttons[i]).removeClass('green-selected');
    }
    $(element).addClass('green-selected');
}

$('.economy-button').on("click", async function () {
    changeColour('.economy-button-label');
    collapseAllAndShow("#economy-collapse");
});

$('.environment-button').on("click", async function () {
    changeColour('.environment-button-label');
    collapseAllAndShow("#environment-collapse");
});

$('.social-button').on("click", async function () {
    changeColour(".social-button-label");
    collapseAllAndShow("#social-collapse");
});

$('.resources-button').on("click", async function () {
    changeColour(".resources-button-label");
    collapseAllAndShow("#resources-collapse");
});