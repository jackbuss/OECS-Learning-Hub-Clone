$('#modal').on('show.bs.modal', function (event) {
    
    var modal = $(this);
    var modalbody = modal.find('.modal-body');
    var modaltitle = modal.find('.modal-title');

    var button = $(event.relatedTarget); // Button that triggered the modal
    var imgsrc, iframesrc, title, target;

    //set the title
    modaltitle.html(button.attr('title'));
    // Load in the content
    modalbody.load(button.attr('href') + '?m=1', function (response, status, xhr) {});
    
});

$('#modal').on('hide.bs.modal', function(event) {
    var modal = $(this);
    var modalbody = modal.find('.modal-body');
    var modaltitle = modal.find('.modal-title');

    modalbody.html("<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span> Loading...");
    modaltitle.text('');
});
