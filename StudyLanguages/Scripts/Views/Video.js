VideoController = {
    Init: function () {
        this.containers = {
            translationToogleBtn: $('#translationToogleBtn'),
            speech: $('#textContainer'),
            showTextBtn: $('#showTextBtn')
        };
        this.translationToogleBtn = new ToggleTranslateBtn({ translated: $('.video-text-translation'), translatedToogle: this.containers.translationToogleBtn });
        
        GlobalBusiness.newVisitor(ServerData.Patterns.UrlNewVisitor);
    },

    ShowHideText: function () {
        var needToHide = this.containers.speech.is(':visible');
        var btnLabel;
        if (needToHide) {
            btnLabel = 'Показать текст <span class="glyphicon glyphicon-arrow-down"></span>';
        } else {
            btnLabel = 'Спрятать текст <span class="glyphicon glyphicon-arrow-up"></span>';
        }
        this.containers.translationToogleBtn.toggle();
        if (needToHide) {
            this.containers.speech.hide();
        } else {
            this.containers.speech.show();
        }
        this.containers.showTextBtn.html(btnLabel);
    },

    ToogleTranslation: function() {
        var result = this.translationToogleBtn.Toogle();
        $(window).resize();
        return result;
    }
};

$(function() {
    VideoController.Init();
});