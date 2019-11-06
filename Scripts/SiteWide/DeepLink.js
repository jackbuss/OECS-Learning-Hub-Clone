; (function (tbDeepLink, undefined) {
    //private
    $(function () {
        $(document).on("click", ".suggestedContent", function (e) {
            var modalbody = $(document).find('.modal-body');
            modalbody.html("<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span> Loading...");
            var toForward = $(this).data("forward");
            modalbody.load(toForward + '?m=1', function (response, status, xhr) {

                if (status != "success") {
                    modalbody.html("<h1>Sorry an error has occurred, please try again later</h1>");
                }
            });
        });
    });
    //public 

})(window.tbDeepLink = window.tbDeepLink || {});