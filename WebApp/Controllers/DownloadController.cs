using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace WebApp.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Index(string url)
        {
            string download = url.Split(new string[] { "google.com/file/d/" }, StringSplitOptions.RemoveEmptyEntries)[1];
            download = @"https://drive.google.com/uc?export=download&id=" + download.Split('/')[0];
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(download);
                using (var ms = new MemoryStream(imageBytes))
                {
                    Image output = Image.FromStream(ms);
                    output.Save(@"C:\Users\SAP-LAP-FPT\Desktop\" + DateTime.Now.Ticks + ".jpg");
                }
            }
            //using (MemoryStream ms = new MemoryStream()
            //{
            //    byte[] buffer = new byte[4096];
            //    int bytesRead = 0;
            //    while ((bytesRead = sr.Read(buffer, 0, buffer.Length)) != 0)
            //    {
            //        dataStream.Write(buffer, 0, bytesRead);
            //    }
            //}
            //WebClient wb = new WebClient();
            //wb.DownloadFile(download, @"C:\Users\SAP-LAP-FPT\Desktop\" + DateTime.Now.Ticks + ".jpg");

            return Redirect("/");
        }

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            Console.Read();

        }
    }
}
