PopularItemsAllController = {
    examples: [],
    
    Init: function() {
        var lastSpoken = null;
        
        UserKnowledge.CheckExistenceIds();

        var elems = ServerData.Elements;
        var autoPronounceCheckBox = new AutoPronounceCheckBox();
        var addAutoPronounce = function (ctrl) {
            var span = ctrl.children('span');
            span.mouseover(function () {
                if (!autoPronounceCheckBox.IsChecked()) {
                    return false;
                }
                var text = $(this).data('text');
                $.each(elems, function(index, elem) {
                    var word = null;
                    if (elem.Source.Text == text) {
                        word = elem.Source;
                    } else if (elem.Translation.Text == text) {
                        word = elem.Translation;
                    }
                    if (word == null || lastSpoken == word) {
                        return true;
                    }
                    lastSpoken = word;
                    window.setTimeout($.proxy(function() {
                        if (lastSpoken != word) {
                            return;
                        }
                        //в течении 0.2 секунды был выделен один и тот же элемент - произнести
                        Speaker.SpeakIfNeed(lastSpoken, ServerData.Patterns.SpeakerType);
                    }, this), 200);
                    return false;
                });
                return false;
            }).mouseleave(function () {
                lastSpoken = null;
                return false;
            });
        };

        var languageObserverHelper = new LanguageObserverHelper({
            action: function (sourceElem, translationElem) {
                addAutoPronounce(sourceElem);
                addAutoPronounce(translationElem);
            }
        });
        var rows = $('.row-with-data');
        $.each(rows, function (index, row) {
            languageObserverHelper.AddRow(row);
        });
    }
};

$(function() {
    PopularItemsAllController.Init();
});