CheckPaymentController = {
    Init: function () {
        var resultContainer = $("#resultContainer");
        var idField = $("#purchasedGoodsIdField");

        function showMessage(success, text) {
            var options = { isSuccess: success, removeLabels: true, container: resultContainer, text: text };
            var msgInner = new Message(options);
            msgInner.Show();
        }

        $('#checkPurchasedGoodsBtn').click(function () {
            resultContainer.html('');
            var uniqueDownloadId = idField.val().trim();
            if (uniqueDownloadId == "") {
                showMessage(false, "Введите идентификатор покупки!");
                return;
            }

            $.getJSON(ServerData.Urls.CheckUrl, { uniqueDownloadId: uniqueDownloadId }, function (response) {
                if (response == null || response.success == false) {
                    showMessage(false, "Товар не оплачен! Скачивание возможно только после оплаты.");
                    return;
                }

                resultContainer.html(response.result).show();
            });
        });
    },
};

$(function() {
    CheckPaymentController.Init();
});