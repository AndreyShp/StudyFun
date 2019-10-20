using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(StudyLanguages.BackloadMVC.AppStart_Initializer), "Initialize")]
 
namespace StudyLanguages.BackloadMVC {
    public static class AppStart_Initializer {
        public static void Initialize() {
            // Use bundeling for client files (scripts (js) and styles (css)). 
            // Comment this out, if you manually include the files in your page
            // Overload: RegisterBundles(BundleTable.Bundles, pathToScripts, pathToStyles) 
            // Example: Backload.Configuration.FileUploadBundles.RegisterBundles(BundleTable.Bundles, "~/Scripts/FileUpload/", "~/Content/FileUpload/css/");
            Backload.Configuration.FileUploadBundles.RegisterBundles(BundleTable.Bundles);
        }
    }
}