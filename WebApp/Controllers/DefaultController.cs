using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPush;

namespace FPT_Science.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public void Index()
        {
            var pushEndpoint = @"https://fcm.googleapis.com/fcm/send/cWm9kR2qJic:APA91bEe0tKuWmmY9HB2zjWD9pDaa3PvwtO7K_vZQSwMjMcdyFArGSJQX-p5Wlg99u6BUlRY4tDPBTAHKdE-vmKb5LUumD6_qVtOLiYQF_ni5pfaTI6rqIpyzuIXry2QwTQ1DpW3H4XC";
            var p256dh = @"BEAJGx-itpRGenXmiFxNjSujZfnKLOD5vRAH8p5WgTjIw3C5dgcQfH8FErIste4v_j9xeUmd_uCR1jJ4fjjqHy0";
            var auth = @"ZvmaG95Vt7tggKaJoW6dhA";

            //Android bugged
            //var pushEndpoint = @"https://fcm.googleapis.com/fcm/send/cI0P54ekjvU:APA91bESOfyswnaof08mQwMwLoh8CHWiw83VlLuiXw0DDBO6w3wcaPYlw4WUdTTZE2wvD-8CR1d1odQA4zRAVNdlQeb6S_QpQJqwkdE6LlY3UNYRfaGLLqXTIktv4dyzNogghLl480Wc";
            //var p256dh = @"BBqB2J71DRMtswIZMVtAgEBhjWeKmrZD_YGpvDL-3sfx9i4NYsHX4jG6uaosRCopGMrfN3TBO3_myUKwM7nselg";
            //var auth = @"OTx21JLBtxSchXcCFUUkyQ";

            var subject = @"mailto:example@example.com";
            var publicKey = @"BMEf2HHXX9p2gOV9FCn5Ds6sLjYFxEoyctGqh73PfzhUSg_I2MuoyxABXR6EWHLBPdVsup61nAfR1-nEQC2yGYM";
            var privateKey = @"5s_tAAVSEViKHYkE_OjhRvkfoWZYg4s2a3yGGgEC72k";

            var subscription = new PushSubscription(pushEndpoint, p256dh, auth);
            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            //var gcmAPIKey = @"[your key here]";

            var message = 
                new JObject
            {
                { "title", "Tin nhắn từ Nguyen" },
                { "body", "Đang trên lab 1 mình nè" }
            };
            //new { title = "Yêu cầu hỗ trợ hội nghị", body = "Đã được chấp nhận" };

            var webPushClient = new WebPushClient();
            try
            {
                webPushClient.SendNotification(subscription, message.ToString(), vapidDetails);
                //webPushClient.SendNotification(subscription, "payload", gcmAPIKey);
            }
            catch (WebPushException exception)
            {
                Console.WriteLine("Http STATUS code" + exception.StatusCode);
            }
        }
    }
}