; (function (tbSearch, undefined) {
    //private
    $(function () {
        $(document).on("click", "#submit-button", function (e) {
            e.preventDefault();
            var form = $(this).closest('form');
            $(form).submit();
        });

        $(document).on("click", "#next-button", function (e) {
            e.preventDefault();
            var form = $(this).closest('form');
            $(this).closest("ul").remove();
            $(form).submit();
        });

        $(document).on("keyup", "#SearchTerm", tbSearch.debounce(function (e) {
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