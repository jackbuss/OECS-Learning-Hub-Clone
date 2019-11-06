$(document).on("click", ".suggestedContent", function (e) {
    var modalbody = $(document).find('.modal-body');
    var toForward = $(this).data("forward");
    modalbody.load(toForward + '?m=1', function (response, status, xhr) { });
});