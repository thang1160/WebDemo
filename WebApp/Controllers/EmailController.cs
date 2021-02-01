using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;

namespace WebApp.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        [HttpPost, ValidateInput(false)]
        [Obsolete]
        public ActionResult Index(string email, string subject, string editor1)
        {
            // Thiết lập phạm vi truy xuất dữ liệu Scope = Drive để upload file
            string[] Scopes = { GmailService.Scope.MailGoogleCom };
            string ApplicationName = "Google Drive API .NET";

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

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            MimeMessage mimeMessage = createEmail(email, "thangdvhe130214@fpt.edu.vn", subject, editor1);

            sendMessage(service, "thangdvhe130214@fpt.edu.vn", mimeMessage);

            return View();
        }

        [Obsolete]
        public static MimeMessage createEmail(string to, string from, string subject, string bodyText)
        {
            MimeMessage email = new MimeMessage();

            email.From.Add(new MailboxAddress(from));
            email.To.Add(new MailboxAddress(to));
            email.Subject = subject;
            email.Body = new TextPart("html")
            {
                Text = bodyText
            };
            return email;
        }

        public static Message createMessageWithEmail(MimeMessage emailContent)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                emailContent.WriteTo(stream);
                byte[] bytes = stream.ToArray();
                string encodedEmail = Convert.ToBase64String(bytes)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", "");
                Message message = new Message();
                message.Raw = encodedEmail;
                return message;
            }
        }

        public static Message sendMessage(GmailService service, string userId, MimeMessage emailContent)
        {
            Message message = createMessageWithEmail(emailContent);
            message = service.Users.Messages.Send(message, userId).Execute();

            Debug.WriteLine("Message id: " + message.Id);
            Debug.WriteLine(message.ToString());
            return message;
        }
    }
}
