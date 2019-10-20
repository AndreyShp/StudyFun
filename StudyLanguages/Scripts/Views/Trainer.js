TrainerController = {
    Init: function() {
        var smartTrainer = new SmartTrainer({
            setMarkUrl: ServerData.GetPath(ServerData.Patterns.Urls.SetMark),
            loop: true
        });
        smartTrainer.SetItems(ServerData.Items);
    }
};

$(function() {
    TrainerController.Init();
});