InnerGroupAllController = {
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
            //UserKnowledge.ShowBtn(row);
            
            languageObserverHelper.AddRow(row);
        });

        /*var addExampleButtonIfNeed = $.proxy(function (container) {
            var id = container.data('id');
            var text = container.data('text');
            $.each(this.examples, function (i, idWithExamples) {
                var found = idWithExamples.Item1 == id;
                if (found) {
                    container.append(
                        '<span class="glyphicon glyphicon-list clickable-element" style="margin-left:5px;" title="Показать примеры"'
                        + ' onclick="InnerGroupAllController.ShowExamples(' + id + ',\'' + text + '\');"></i>');
                }
                return !found;
            });
        }, this);

        var url = ServerData.GetPath(ServerData.Patterns.ExamplesUrl);
        $.get(url, $.proxy(function(data) {
            if (!data) {
                return;
            }
            this.examples = data;
            $.each(rowsInfo, function (index, rowInfo) {
                addExampleButtonIfNeed(rowInfo.sourceElem);
                addExampleButtonIfNeed(rowInfo.translationElem);
            });
        }, this));*/

        GlobalBusiness.newVisitor(ServerData.Patterns.UrlNewVisitor);
    },

    /*,
    
    ShowExamples: function (id, text) {
        $.each(this.examples, function (i, idWithExamples) {
            var found = idWithExamples.Item1 == id;
            if (found) {
                var header = String.format('Примеры использования {0} \"{1}\":', ServerData.Patterns.LowerManyItems, text);
                GlobalBusiness.showExamplesInWindow(header, idWithExamples.Item2);
            }
            return !found;
        });
    }*/
};

$(function() {
    InnerGroupAllController.Init();
});