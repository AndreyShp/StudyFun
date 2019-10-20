RepresentationController = {
    Init: function () {
        var getUrl = function(action) {
            return ServerData.GetPath("/Admin/Representation/" + action);
        };

        var containers = {
            fileUpload: '#fileupload'        
        };
        
        var url = getUrl('Upload');
        debugger;   //????
        // Initialize the jQuery File Upload widget:
        $('#fileupload').fileupload({
            // Uncomment the following to send cross-domain cookies:
            //xhrFields: {withCredentials: true},
            url: url
        });
    }
};

$(function() {
    RepresentationController.Init();
});