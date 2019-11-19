; (function (tbDeepLink, undefined) {
    //private
    var modalbody = $('#modal').find('.modal-body');

    $(function () {
        $(document).on("click", "a.suggestedContent", function (e) {
            e.preventDefault();
            modalbody.html("<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span> Loading...");
            var toForward = $(this).attr("href");

            modalbody.load(toForward + '?m=1', function (response, status, xhr) {

                if (status !== "success") {
                    modalbody.html("<h1>Sorry an error has occurred, please try again later</h1>");
                }
            });
        });

        //todo: change to querystring so we can rewrite rather than redirect?
        if (location.hash) {
            const hash = location.hash;
            let toForward = hash.replace(/#/, "");
            if (toForward !== 'undefined') {
                $('#modal').modal('show');
                modalbody.load(toForward + '?m=1', function (response, status, xhr) {
                    if (status !== "success") {
                        modalbody.html("<h1>Sorry an error has occurred, please try again later</h1>");
                    }
                });

                history.replaceState(null, null, toForward);
                setTimeout(() => {
                    $(window).scrollTop(0);
                }, 400);
            }
        }

        $(document).on("click", 'a[data-toggle="modal"], a.suggestedContent', function () {
            history.pushState(null, null, $(this).attr("href"));
        });

        $(window).on('popstate', function (e) {
            let toForward = location.pathname;
            //
            if (toForward !== '/') {
                $('#modal').modal('show');
                //
                modalbody.load(toForward + '?m=1', function (response, status, xhr) {
                    if (status !== "success") {
                        modalbody.html("<h1>Sorry an error has occurred, please try again later</h1>");
                    }
                });
            } else {
                $('#modal').modal('hide');
            }
        });

        $('#modal').on('hidden.bs.modal', function (event) {
            history.pushState(null, null, '/');
        });
    });
    //public 

})(window.tbDeepLink = window.tbDeepLink || {});