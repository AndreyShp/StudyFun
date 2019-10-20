KnowledgeWall = {
    MEDIA_SELECTOR: '.media',
    
    Init: function () {
        var prevId = null;
        var medSel = this.MEDIA_SELECTOR;
        var languageObserverHelper = new LanguageObserverHelper();

        //NOTE: оставить конструкцию ниже по привязке событий, т.к. привязка событий не работает к элементам, которые динамически добавляются на страницу(фоновая подгрузка данных)
        /*$(document).on('mouseover', medSel, function () {
            var removeBtn = $(this).find('.knowledge-wall-remove-btn');
            removeBtn.css('visibility', 'visible');
        }).on('mouseleave', medSel, function () {
            var removeBtn = $(this).find('.knowledge-wall-remove-btn');
            removeBtn.css('visibility', 'hidden');
        });*/

        var medias = $(medSel);
        $.each(medias, function(index, media) {
            var id = $(media).data('id');
            if (id && (id < prevId || !prevId)) {
                prevId = id;
            }
            languageObserverHelper.AddRow(media);
        });

        var hasNoData = false;
        var isLoading = false;
        var loadData = function() {
            if (hasNoData) {
                //данных больше нет
                return;
            }
            if (isLoading) {
                //уже подгружаем данные
                return;
            }

            //TODO: отобразить загрузку
            isLoading = true;
            var url = ServerData.GetPath(ServerData.Patterns.Urls.LoadingData);
            $.getJSON(url, { prevId: prevId }, function (response) {
                if (response == null || response.success === false) {
                    PopupAlert.Show('Не удалось подгрузить данные. Попробуйте прокрутить страницу вверх и вниз');
                    isLoading = false;
                    return;
                }

                var items = response.items;
                if (items.length == 0) {
                    hasNoData = true;
                }

                var sourceLanguageId = response.sourceLanguageId;
                var itemsContainer = $('#' + ServerData.Patterns.ItemsContainerId);
                $.each(items, function (index, item) {
                    var media = $(item);
                    var itemId = media.data('id');
                    if (itemId < prevId) {
                        prevId = itemId;
                    }
                    itemsContainer.append(media);
                    
                    //NOTE: не убирать повторный поиск, т.к. без этого не работает
                    var appendedMedia = itemsContainer.find(medSel + ':last');

                    languageObserverHelper.AddRow(appendedMedia, sourceLanguageId);
                });
                isLoading = false;
            });
        };

        $(window).scroll(function() {
            var scrollPos = $(window).scrollTop() + $(window).height();
            var height = $(document).height();
            var maxDistance = $(window).height();

            if (scrollPos < height - maxDistance) {
                return;
            }
            loadData();
        });
        
        var hasScroll = $("body").height() > $(window).height();
        if (!hasScroll) {
            //скрола нет - подгрузить следующую порцию
            loadData();
            //TODO: может быть баг, что после подгрузки скролл может не появиться - тогда нужно загружать еще до тех пор, пока скролл не появится или данные не закончатся
        }
    },
    
    Remove: function (ctrl) {
        var block = $(ctrl).parents(this.MEDIA_SELECTOR);
        var id = block.data('id');
        if (!id) {
            return;
        }
        
        var contentBlock = block.find('.knowledge-wall-content');
        contentBlock.hide();
        var restoreBlock = block.find('.knowledge-wall-restore');

        var removeOrRestore = function (options) {
            var url = ServerData.GetPath(ServerData.Patterns.Urls.RemoveOrRestore);
            $.post(url, { id: id, needRemove: options.needRemove }, function (data) {
                var success = data != null && data.success;
                if (!success) {
                    PopupAlert.Show(options.errorMessage);
                    return;
                }
                options.success();
            }, "json");
        };

        removeOrRestore({
            needRemove: true,
            success: function() {
                restoreBlock.show().unbind('click').click(function () {
                    var restoreOptions = {
                        needRemove: false,
                        success: function () {
                            contentBlock.show();
                            restoreBlock.hide();
                        },
                        errorMessage: 'Не удалось восстановить порцию знаний! Попробуйте позже.'
                    };
                    removeOrRestore(restoreOptions);
                });
            },
            errorMessage: 'Не удалось удалить порцию знаний! Попробуйте позже.'
        });
    }
};

$(function() {
    KnowledgeWall.Init();
});