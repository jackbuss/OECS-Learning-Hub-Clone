// watch for picture load from sourceset changes and change backgrounds
; (function (picturefillBG, undefined) {

    $(function () {
        $('.banner-image').each(function () {

            $img = $(this).find('picture.bg-picture>img');

            $img.on("load", function () {
                var src = typeof this.currentSrc !== 'undefined' ? this.currentSrc : this.src;
                $(this).closest('div.banner-image').css('background-image', 'url(' + src + ')');
            }).each(function () {
                if (this.complete) {
                    $(this).trigger('load'); // For jQuery >= 3.0 
                }
            });            

        });
    });

})(window.picturefillBG = window.picturefillBG || {});