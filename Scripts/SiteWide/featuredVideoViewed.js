
console.log("hello");


$('#modal').on('show.bs.modal', function (event) {


    var modal = $(this);
    var modalbody = modal.find('.modal-body');

    //var iframe = modalbody.find('.embed-responsive-item');

    if (modal.find(".embed-responsive-item").length > 0) {
        alert("found iframe");
    }
    
});

$(document).on("click", ".play", function () {
    console.log("play button clicked");
});

//injectFinish();

//var iframe = $(".embed-responsive-item");
//var player = new Vimeo.Player(iframe);

//player.on('ended', function () {
//    console.log("Video has finished");
//    injectFinish();
//    //TODO: inject cookie
//});

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
