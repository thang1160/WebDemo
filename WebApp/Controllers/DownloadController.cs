using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;

namespace WebApp.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Index(string url)
        {
            // Thiết lập phạm vi truy xuất dữ liệu Scope = Drive để upload file
            string[] Scopes = { DriveService.Scope.Drive };
            string ApplicationName = "Google Drive API .NET";

            UserCredential credential;
            using (var stream = new FileStream(HostingEnvironment.MapPath("/credentials.json"), FileMode.Open, FileAccess.Read))
            {

                // Thông tin về quyền truy xuất dữ liệu của người dùng được lưu ở thư mục token.json
                string credPath = "token.json";

                // Yêu cầu người dùng xác thực lần đầu và thông tin sẽ được lưu vào thư mục token.json
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,  // Quyền truy xuất dữ liệu của người dùng
                    "user",
                    CancellationToken.None).Result;

                Console.WriteLine("Credential file saved to: " + credPath);
            }


            // Tạo ra 1 dịch vụ Drive API - Create Drive API service với thông tin xác thực và ApplicationName

            var driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // ID thư mục file, các bạn thay bằng ID của các bạn khi chạy
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                // Tên file sẽ lưu trên Google Drive
                Name = DateTime.Now.Ticks + ".jpg",
                Parents = new List<string>
                {
                    "1heLB1zJxTqq0UrZ8sPn-seoyThzZ-hUW"
                }
            };

            string download = url.Split(new string[] { "google.com/file/d/" }, StringSplitOptions.RemoveEmptyEntries)[1];
            download = @"https://drive.google.com/uc?export=download&id=" + download.Split('/')[0];
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(download);
                using (var ms = new MemoryStream(imageBytes))
                {
                    FilesResource.CreateMediaUpload request = driveService.Files.Create(fileMetadata, ms, "image/jpeg");
                    // Cấu hình thông tin lấy về là ID
                    request.Fields = "id";
                    request.SupportsAllDrives = true;
                    var result = request.Upload();
                    Debug.WriteLine(result.Status);

                    // Trả về thông tin file đã được upload lên Google Drive
                    Google.Apis.Drive.v3.Data.File file = request.ResponseBody;

                    Debug.WriteLine("File ID: " + file.Id);
                }
            }

            credential.RevokeTokenAsync(new CancellationToken());

            return Redirect("/");
        }
    }
}
