
console.log("hello"); // check if script is being called properly


$('#modal').on('shown.bs.modal', function (event) {

    alert("modal opened");

    var modal = $(this);
    var modalbody = modal.find('.modal-body');

    //var iframe = modalbody.find('.embed-responsive-item');

    var player = modal.find(".vp-center");

    //if ($(".vp-center")) {
    //    alert("Player found");
    //}

    
});

$(document).on("click", ".play", function () {
    console.log("play button clicked");
});

//injectFinish();

var iframe = $(".embed-responsive-item");
var player = new Vimeo.Player(iframe);

player.on('ended', function () {
    alert("Video has finished");
    //injectFinish();
    //TODO: inject cookie
});

function injectCookies(ids) {
    document.cookie = "viewedVideos=" + ids;
}

function injectFinish() {
    var tileId;
    var url = window.location.href;

    if ($(".modal").is(":visible")) {
        console.log("modalvisible");
    } else {
        console.log("modal not visible");
    }
    console.log(url);
}
