ComparisonController = {
    COUNT_ELEMENTS_IN_ROW: 2,
    ELEMENTS_CLASS: '.partial-comparison-item-container',
    rows: [],

    Init: function () {
        UserKnowledge.CheckExistenceIds();
        
        GlobalBusiness.newVisitor(ServerData.Patterns.UrlNewVisitor);
        this.elems = $.makeArray($(this.ELEMENTS_CLASS));
        $.each(this.elems, $.proxy(function (index, elem) {
            var row = $(elem).parent('.row');
            if (index % this.COUNT_ELEMENTS_IN_ROW == 0) {
                this.rows.push(row);
            }
        }, this));

        /*$.each($('[data-example-container]'), function (index, exampleContainer) {
            UserKnowledge.ShowBtn(exampleContainer);
        });*/
    },
    
    HideOrShow: function (itemId) {
        var item = $(this.elems).filter('#' + itemId).first();
        var isVisible = item.is(':visible');
        var checkBoxId = '#' + String.format(ServerData.Patterns.CheckBoxId, itemId);
        if (isVisible) {
            $(checkBoxId).removeAttr('checked');
            item.hide();
        } else {
            $(checkBoxId).attr('checked', true);
            item.show();
        }

        this.reorderIfNeed();
        return false;
    },
    
    reorderIfNeed: function () {
        if (this.rows <= 1) {
            //слишком мало - нечего переупорядочивать
            return;
        }

        var index = 0;
        var visibleElements = $(this.elems).filter($.proxy(function(i) {
            return this.elems[i].style['display'] != 'none';
        }, this));
        var countElemsInRow = this.COUNT_ELEMENTS_IN_ROW;
        $.each(this.rows, function (i, row) {
            row.children().filter(this.ELEMENTS_CLASS).remove();
            if (index >= visibleElements.length) {
                return;
            }

            var countElements = visibleElements.length - index;
            if (countElements > countElemsInRow) {
                countElements = countElemsInRow;
            }
            var elementsForCurrentRow = visibleElements.slice(index, index + countElements);
            index += countElements;
            row.append(elementsForCurrentRow);
        });
    }
};

$(function() {
    ComparisonController.Init();
});