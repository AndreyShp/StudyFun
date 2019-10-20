using Backload.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Backload;

namespace StudyLanguages.Controllers
{
    // NOTE: By default none of these 2 controllers handles the file upload, because its handled by the file upload 
    // controller/handler integrated into Backload. So you do not have to setup your own controllers, unless you want 
    // to have the full controll. To use your own controller, you have to set the url of the jQuery File Upload Plugin (main.js) 
    // to point to your controller: e.g. url: '/FileUploadDerived/UploadHandler'.

    // Method 1: Derive from BackloadController and call the base classe
    public class FileUploadDerivedController : BackloadController
    {
        public override JsonResult UploadHandler()
        {
            // Call base class to handle the file upload
            JsonResult result = base.HandleRequest();

            return result;
        }
    }

    // Method 2: Use a common controller, instantiate a FileUploadHandler object 
    // from BackloadController and call HandleRequest(Request) on it.
    // NOTE: In this example we registered an event handler to cancel a file upload.
    public class FileUploadInstanceController : Controller
    {
        public JsonResult UploadHandler()
        {
            FileUploadHandler handler = new FileUploadHandler(Request);
            handler.FileUploadedStarted += FileUploadedStarted;
            JsonResult result = handler.HandleRequest();

            return result;
        }

        void FileUploadedStarted(object sender, Backload.Eventing.UploadStartedEventArgs e)
        {
            // Example: Prevent files of type jpeg to be processed
            if (e.ContentType == "image/jpeg")
            {
                // Cancel processing of the incoming file
                e.CancelAction = true;
                e.CancelMessage = "Upload canceled, File type not accepted";
            }
        }
    }
}
