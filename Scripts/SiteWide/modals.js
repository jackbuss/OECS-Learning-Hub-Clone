$('#modal').on('show.bs.modal', function (event) {

    var modal = $(this);
    var modalbody = modal.find('.modal-body');
    var modaltitle = modal.find('.modal-title');

    var button = $(event.relatedTarget); // Button that triggered the modal
    var imgsrc, iframesrc, title, target;

    //set the title
    modaltitle.html(button.attr('title'));
    // Load in the content
    modalbody.load(button.attr('href') + '?m=1', function (response, status, xhr) {
        if (status == "success") {

            if (modalbody.find(".embed-responsive-item").length > 0) {
                var tileId = button.data("key");

                var iframe = $(".embed-responsive-item");
                if (iframe.attr("src").includes("vimeo.com")) {
                    var player = new Vimeo.Player(iframe);

                    player.on('ended', function () {
                        injectEnd(tileId);
                    });
                } else if (iframe.attr("src").includes("youtube.com")) {



                }

            }

        } else {
            modalbody.html("<h1>Sorry an error has occurred, please try again later</h1>");
        }
    });

});

function injectEnd(id) {
    var cookie = getCookie("viewedVideos"); // get existing cookie
    if (cookie == "") {
        setCookie("viewedVideos", id, 30);
    } else if (!(cookie.includes(id))) {
        setCookie("viewedVideos", cookie + "," + id, 30);
    }

}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

$('#modal').on('hide.bs.modal', function (event) {
    var modal = $(this);
    var modalbody = modal.find('.modal-body');
    var modaltitle = modal.find('.modal-title');

    modalbody.html("<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span> Loading...");
    modaltitle.text('');
});
