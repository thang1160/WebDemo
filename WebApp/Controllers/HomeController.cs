using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;

namespace FPT_Science.Controllers
{
    public class HomeController : Controller
    {
        readonly string[] Scopes = { DriveService.Scope.DriveFile };
        public ActionResult Index()
        {

            UserCredential credential;

            using (var stream =
                new FileStream(HostingEnvironment.MapPath("/credentials.json"), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None).Result;

                //credential.RevokeTokenAsync(new CancellationToken());
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Reset()
        {
            UserCredential credential;

            using (var stream =
                new FileStream(HostingEnvironment.MapPath("/credentials.json"), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None).Result;

                bool a = credential.RevokeTokenAsync(new CancellationToken()).Result;
            }

            return Redirect("/");
        }
    }
}
