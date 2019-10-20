KnowledgeGenerator = {
    Generate: function () {
        var url = ServerData.GetPath(ServerData.Patterns.Urls.LoadingData);
        $.getJSON(url, null, function (response) {
            if (response == null || !response.length) {
                PopupAlert.Show('Не удалось сгенерировать данные. Попробуйте еще раз');
                return;
            }

            var itemsContainer = $('#' + ServerData.Patterns.ItemsContainerId);
            itemsContainer.empty();
            $.each(response, function (index, item) {
                itemsContainer.append(item);
            });
        });

        return false;
    }
};

/*$(function() {
    KnowledgeGenerator.Init();
});*/