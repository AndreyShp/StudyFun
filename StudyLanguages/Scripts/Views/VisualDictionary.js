VisualDictionaryController = {
    Init: function () {
        var IMAGE_CONTAINER = '#image';
        
        UserKnowledge.CheckExistenceIds();

        this.translationToogleBtn = new ToggleTranslateBtn({ translated: $('.visual-translate-word-container'), translatedToogle: $('#translationToogleId') });
        this.areas = ServerData.Areas;
        this.lastSpoken = null;

        var autoPronounceCheckBox = new AutoPronounceCheckBox();
        this.speakIfNeed = function (area) {
            var isChecked = autoPronounceCheckBox.IsChecked();
            Speaker.SpeakElemPartByLanguage(isChecked, area, ServerData.SpeakerHelper.Ids.Word);
        };

        //эта функция будет вызывана всегда при изменении области выделения/снятия области выделения
        var changeWordSelection = $.proxy(function (isSelected, area, cleanSpeak) {
            var VISUAL_UNSELECTED_CLASS = 'visual-unselected-word';
            var isFound = false;
            if (cleanSpeak) {
                this.lastSpoken = null;
            }
            $.each(this.areas, $.proxy(function (index, a) {
                if (isSelected && a != area) {
                    a.wordContainer.addClass(VISUAL_UNSELECTED_CLASS);
                } else {
                    a.wordContainer.removeClass(VISUAL_UNSELECTED_CLASS);
                }
                isFound |= a == area;
                return true;
            }, this));
            if (!isSelected || !isFound || this.lastSpoken == area) {
                return;
            }
            this.lastSpoken = area;
            window.setTimeout($.proxy(function () {
                if (this.lastSpoken != area) {
                    return;
                }
                //в течении 0.2 секунды был выделен один и тот же элемент - произнести
                this.speakIfNeed(this.lastSpoken);
            }, this), 200);
            
        }, this);

        this.changeLanguageObserver = $.proxy(function(sourceLanguageId) {
            var setWord = function (elemId, area, word) {
                var text = Speaker.GetWordHtml(word);
                $(String.format(elemId, area.Id)).html(text);
            };

            $.each(this.areas, function(index, area) {
                var source;
                var translated;
                if (area.Source.LanguageId == sourceLanguageId) {
                    source = area.Source;
                    translated = area.Translation;
                } else {
                    source = area.Translation;
                    translated = area.Source;
                }

                setWord('#sourceWord_{0}', area, source);
                setWord('#translatedWord_{0}', area, translated);
            });
            
            $(window).resize();
        }, this);

        //подписаться на изменение языка
        LanguagePanel.AddObserver($.proxy(this.changeLanguageObserver, this));

        var getAreaTitle = $.proxy(function (area) {
            var source;
            var translation;
            var sourceLanguageId = LanguagePanel.GetSourceLanguageId();
            if (area.Source.LanguageId == sourceLanguageId) {
                source = area.Source;
                translation = area.Translation;
            } else {
                translation = area.Source;
                source = area.Translation;
            }

            var title = source.Text;
            if (this.translationToogleBtn.IsVisible()) {
                title += ' - ' + translation.Text;
            }
            return title;
        }, this);

        var clickArea = function (area) {
            Speaker.SpeakElemPartByLanguage(true, area, ServerData.SpeakerHelper.Ids.Word);
        };

        var createAreasSelector = function(areas) {
            var result = new MultiImageAreasSelector({
                imageContainer: IMAGE_CONTAINER,
                originalImageWidth: ServerData.Size.Width,
                originalImageHeight: ServerData.Size.Height,
                onChangeSelection: changeWordSelection,
                getAreaTitle: getAreaTitle,
                onClickArea: ServerData.CanPronounce ? clickArea : null
            });
            $.each(areas, function (index, area) {
                //в качестве id, использую целый объект area
                result.AddArea(area, area.LeftUpperCorner.X, area.LeftUpperCorner.Y, area.RightBottomCorner.X, area.RightBottomCorner.Y);

                area.wordContainer = $(String.format('#wordContainer_{0}', area.Id));
                area.wordContainer
                    .mouseleave(function () {
                        result.CleanSelection(true);
                    }).mouseover(function () {
                        //в качестве id, использую целый объект area
                        result.ShowSelection(area);
                    });
                //UserKnowledge.ShowBtn(area.wordContainer);
            });
            return result;
        };
        
        $(IMAGE_CONTAINER).load(function () {
            if (!$(window).resize) {
                return;
            }
            $(window).resize();
        });

        var areasSelector = createAreasSelector(this.areas);
        $(window).resize($.proxy(function () {
            areasSelector.Clear();
            areasSelector = createAreasSelector(this.areas);
        }, this));

        GlobalBusiness.newVisitor(ServerData.Patterns.UrlNewVisitor);
    },

    ToogleTranslation: function() {
        var result = this.translationToogleBtn.Toogle();
        $(window).resize();
        return result;
    }
};

$(function() {
    VisualDictionaryController.Init();
});

var MultiImageAreasSelector = function (options) {
    var SELECTED_CLASS = 'multi-image-selected-area';
    var AREA_CLASS = 'multi-image-area';
    var DARK_CLASS = 'multi-image-dark-area';
        
    var isBuildZIndex = false;
    var buildZIndex = function (areas) {
        var sortSquare = function (a, b) {
            return b.square - a.square;
        };
        $.each(areas.sort(sortSquare), function (index, a) {
            a.areaCtrl.css('zIndex', index);
        });
    };

    var image = $(options.imageContainer);
    var divAppender = function(className, left, top, width, height) {
        var content = String.format(
                   "<div class='{0}' style='left:{1}px;top:{2}px;width:{3}px;height:{4}px;'></div>",
                   className, left, top, width, height);
        return $(content).insertAfter(image);
    };

    var onChangeSelection = options.onChangeSelection;
    var getAreaTitle = options.getAreaTitle;
    var onClickArea = options.onClickArea;
    
    var scaleXFactor = image.width() / options.originalImageWidth;
    var scaleYFactor = image.height() / options.originalImageHeight;

    this.areas = [];
    var imagePosition = image.position();

    this.darkAreas = [];
    this.selectedAreas = [];
    this.AddArea = function (id, relX1, relY1, relX2, relY2) {
        var x1 = parseInt(imagePosition.left + relX1 * scaleXFactor);
        var y1 = parseInt(imagePosition.top + relY1 * scaleYFactor);
        var x2 = parseInt(imagePosition.left + relX2 * scaleXFactor);
        var y2 = parseInt(imagePosition.top + relY2 * scaleYFactor);

        var width = x2 - x1;
        var height = y2 - y1;
        var areaCtrl = divAppender(AREA_CLASS, x1, y1, width, height);
        
        areaCtrl.mouseleave($.proxy(function () {
            this.CleanSelection(true);
        }, this));
        areaCtrl.mouseover($.proxy(function() {
            this.ShowSelection(id);
        }, this));

        if (onClickArea != null) {
            areaCtrl.css('cursor', 'pointer');
            areaCtrl.click($.proxy(function() {
                onClickArea(id);
            }, this));
        }

        var area = {
            id: id,
            coordinates: { x1: relX1, y1: relY1, x2: relX2, y2: relY2 },
            absoluteCoordinates: { x1: x1, y1: y1, x2: x2, y2: y2 },
            areaCtrl: areaCtrl,
            square: width * height
        };
        this.areas.push(area);
    };
    
    this.ShowSelection = function (id) {
        if (!isBuildZIndex) {
            isBuildZIndex = true;
            //построить z-index, чтобы самые маленькие области были выше больших, иначе большие области будут перекрывать маленькие области
            buildZIndex(this.areas);
        }

        this.CleanSelection();
        
        var selected = this.selectedAreas;
        $.each(this.areas, function (index, area) {
            var isFound = id == area.id;
            if (isFound) {
                selected.push(area);
                area.areaCtrl.addClass(SELECTED_CLASS);
                var title = getAreaTitle(area.id);
                area.areaCtrl.attr('title', title);
                onChangeSelection(true, area.id);
            }
            return !isFound;
        });

        var imgX1 = parseInt(imagePosition.left);
        var imgY1 = parseInt(imagePosition.top);
        var iw = parseInt(image.width());
        var ih = parseInt(image.height());

        var darkAreaAppender = $.proxy(function(x, y, w, h) {
            divAppender(DARK_CLASS, x, y, w, h);
        }, this);
        
        //затемнение невыделенных областей
        $.each(this.selectedAreas, function(index, area) {
            var c = area.absoluteCoordinates;
            var marginLeft = c.x1 - imgX1;
            var marginTop = c.y1 - imgY1;
            var marginRight = imgX1 + iw - c.x2;
            var marginBottom = imgY1 + ih - c.y2;

            if (marginLeft > 0) {
                //нужно рисовать слева
                darkAreaAppender(imgX1, c.y1, marginLeft + 1, c.y2 - c.y1 + 1);
            }
            
            if (marginTop > 0) {
                //нужно рисовать сверху
                darkAreaAppender(imgX1, imgY1, iw, marginTop);
            }
            
            if (marginRight > 0) {
                //нужно рисовать справа
                darkAreaAppender(c.x2 + 1, c.y1, marginRight - 1, c.y2 - c.y1 + 1);
            }
            
            if (marginBottom > 0) {
                //нужно рисовать снизу
                darkAreaAppender(imgX1, c.y2 + 1, iw, marginBottom);
            }
        });
    };

    var removeElemsWithClass = function (cls) {
        $(String.format('.{0}', cls)).remove();
    };

    this.CleanSelection = function (cleanSpeak) {
        removeElemsWithClass(DARK_CLASS);

        $.each(this.selectedAreas, function (index, area) {
            area.areaCtrl.removeClass(SELECTED_CLASS);
            onChangeSelection(false, area.id, cleanSpeak);
            return true;
        });
        this.selectedAreas = [];
    };

    this.Clear = function () {
        removeElemsWithClass(DARK_CLASS);
        removeElemsWithClass(AREA_CLASS);
    };
};