; (function (tbCookie, undefined) {
    //private

    function checkConsentedCookies() {
        var nameEQ = "cookies=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    $('#cookieModal').on('show.bs.modal', function (event) {
        var modal = $(this);
        if (checkConsentedCookies() != null) {
            if (checkConsentedCookies().includes("googleAnalytics")) {
                $('#googleAnalytics').prop('checked', true);
            }
            if (checkConsentedCookies().includes("hotjar")) {
                $('#hotjar').prop('checked', true);
            }
        }
    });

    function cookiePolicyAgree(cookies) {
        document.cookie = "cookies=" + cookies;
    }

    $(document).on("click", "#submitCookiePreference", function (e) {
        document.cookie = "cookies=; expires=Thu, 01 Jan 1970 00:00:01 GMT;'";
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
            console.log("I see you google");
            $("body").append("<script id=\"gAnalytics1\" async src=\"https://www.googletagmanager.com/gtag/js?id=UA-148482803-1\"></script><script id=\"gAnalytics2\" language=\"javascript\" src=\"~/Scripts/GoogleAnalytics.js\"></script>");
            console.log("Appended google analytics to body");
        } else {
            $('#gAnalytics1').remove();
            $('#gAnalytics2').remove();
        }

        if (checked.includes("hotjar")) {
            console.log("I see you hotjar");
            // Inject hotjar content
        } else {
            // Remove hotjar content (if there)
        }

    });

    //public 
    // tbCookie.function(){}
})(window.tbCookie = window.tbCookie || {});