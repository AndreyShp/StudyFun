GroupsController = {
    Init: function () {
        //подгружаем остальные изображения - фоном
        setTimeout(function() {
            var imagesToLoad = $('img').not('[data-url-to-load=""]');
            $.each(imagesToLoad, function(index, imageToUrl) {
                var img = $(imageToUrl);
                var newUrl = img.attr('data-url-to-load');
                img.attr('src', newUrl);
            });
        }, 1000);
    }
};

$(function() {
    GroupsController.Init();
});