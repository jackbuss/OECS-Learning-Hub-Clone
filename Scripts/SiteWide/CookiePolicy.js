; (function (tbCookie, undefined) {
    //private

    function cookiePolicyAgree(cookies) {
        document.cookie = "cookies=" + cookies;
    }

    $(document).on("click", "#submitCookiePreference", function (e) {
        var checked = "";
        var element = document.getElementsByClassName("cookieCheckbox");

        for (var i = 0; i < element.length; i++) {
            if (element[i].checked) {
                if (checked == "") {
                    checked += element[i].id;
                } else {
                    checked += "," + element[i].id;
                }
            }
        }

        cookiePolicyAgree(checked);

        if (checked.includes("googleAnalytics")) {
            $("body").append("");
        }

    });

    //public 
    // tbCookie.function(){}
})(window.tbCookie = window.tbCookie || {});