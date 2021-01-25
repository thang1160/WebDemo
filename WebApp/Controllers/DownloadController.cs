using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;

namespace WebApp.Controllers
{
    public class DownloadController : Controller
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";

        // GET: Download
        public ActionResult Index(string url)
        {
            UserCredential credential;

            string directory = HostingEnvironment.MapPath("/credentials.json");

            using (var stream =
                new FileStream(directory.ToString(), FileMode.Open, FileAccess.Read))
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
                Debug.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            Google.Apis.Drive.v3.Data.File fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = DateTime.Now.Ticks + ".jpg",
                MimeType = "image/jpeg"
            };

            string download = url.Split(new string[] { "google.com/file/d/" }, StringSplitOptions.RemoveEmptyEntries)[1];
            download = @"https://drive.google.com/uc?export=download&id=" + download.Split('/')[0];
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(download);
                using (var ms = new MemoryStream(imageBytes))
                {
                    FilesResource.CreateMediaUpload request = service.Files.Create(fileMetadata, ms, "image/jpeg");
                    request.SupportsAllDrives = true;
                    request.ResponseReceived += Request_ResponseReceived;
                    IUploadProgress result = request.Upload();
                    Debug.WriteLine(result.Status);
                    if (result.Status != UploadStatus.Completed)
                    {
                        var rdn = new Random();
                        var waitTime = 0;
                        var count = 0;
                        do
                        {
                            waitTime = (Convert.ToInt32(Math.Pow(2, count)) * 1000) + rdn.Next(0, 1000);
                            Thread.Sleep(waitTime);

                            result = request.Upload();
                            count++;

                        } while (count < 5 && (result.Status != UploadStatus.Completed));
                    }//end solution

                    Google.Apis.Drive.v3.Data.File file = request.ResponseBody;
                    Debug.WriteLine("File was uploaded sucessfully--" + file.Id);
                    //output.Save(@"C:\Users\SAP-LAP-FPT\Desktop\" + DateTime.Now.Ticks + ".jpg");
                }
            }


            // Define parameters of request.
            //FilesResource.ListRequest listRequest = service.Files.List();
            //listRequest.PageSize = 10;
            //listRequest.Fields = "nextPageToken, files(id, name)";

            //// List files.
            //IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
            //    .Files;
            //Debug.WriteLine("Files:");
            //if (files != null && files.Count > 0)
            //{
            //    foreach (var file in files)
            //    {
            //        Debug.WriteLine("{0} ({1})", file.Name, file.Id);
            //    }
            //}
            //else
            //{
            //    Debug.WriteLine("No files found.");
            //}

            return Redirect("/");
        }

        private void Request_ResponseReceived(Google.Apis.Drive.v3.Data.File obj)
        {
            if (obj != null)
            {
                Debug.WriteLine("File was uploaded sucessfully--" + obj.Id);
            }
        }
    }
}
