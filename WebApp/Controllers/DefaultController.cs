using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using WebApp.Models;
using WebPush;

namespace FPT_Science.Controllers
{
    public class DefaultController : Controller
    {
        private SubscribeEntities db = new SubscribeEntities();
        // GET: Default
        public void Index()
        {
            //Android bugged
            //var pushEndpoint = @"https://fcm.googleapis.com/fcm/send/cI0P54ekjvU:APA91bESOfyswnaof08mQwMwLoh8CHWiw83VlLuiXw0DDBO6w3wcaPYlw4WUdTTZE2wvD-8CR1d1odQA4zRAVNdlQeb6S_QpQJqwkdE6LlY3UNYRfaGLLqXTIktv4dyzNogghLl480Wc";
            //var p256dh = @"BBqB2J71DRMtswIZMVtAgEBhjWeKmrZD_YGpvDL-3sfx9i4NYsHX4jG6uaosRCopGMrfN3TBO3_myUKwM7nselg";
            //var auth = @"OTx21JLBtxSchXcCFUUkyQ";

            var subject = @"mailto:example@example.com";
            var publicKey = @"BMEf2HHXX9p2gOV9FCn5Ds6sLjYFxEoyctGqh73PfzhUSg_I2MuoyxABXR6EWHLBPdVsup61nAfR1-nEQC2yGYM";
            var privateKey = @"5s_tAAVSEViKHYkE_OjhRvkfoWZYg4s2a3yGGgEC72k";

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            //var gcmAPIKey = @"[your key here]";

            var message = new JObject
            {
                { "title", "Tin nhắn từ Nguyen" },
                { "body", "Đang trên lab 1 mình nè" }
            };
            //new { title = "Yêu cầu hỗ trợ hội nghị", body = "Đã được chấp nhận" };

            var webPushClient = new WebPushClient();

            List<Subscriber> subs = db.Subscribers.ToList();

            List<Subscriber> gone = new List<Subscriber>();

            foreach (Subscriber item in subs)
            {
                try
                {
                    var subscription = new PushSubscription(item.pushEndpoint, item.p256dh, item.auth);

                    webPushClient.SendNotification(subscription, message.ToString(), vapidDetails);
                    //webPushClient.SendNotification(subscription, "payload", gcmAPIKey);
                }
                catch (WebPushException exception)
                {
                    if (exception.StatusCode == System.Net.HttpStatusCode.Gone)
                    {
                        gone.Add(item);
                    }
                    Console.WriteLine("Http STATUS code" + exception.StatusCode);
                }
            }

            if (gone.Count != 0)
            {
                db.Subscribers.RemoveRange(gone);
                db.SaveChanges();
            }
        }
    }
}
