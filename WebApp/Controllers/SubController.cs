using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using WebApp.Models;

namespace FPT_Science.Controllers
{
    public class SubController : Controller
    {
        // GET: Sub
        public ActionResult Index(string json)
        {
            SubscribeEntities db = new SubscribeEntities();
            JObject _json = JObject.Parse(json);

            string endpoint = _json.Value<string>("endpoint");
            if (db.Subscribers.Find(endpoint) == null)
            {
                JObject keys = _json.Value<JObject>("keys");
                string p256dh = keys.Value<string>("p256dh");
                string auth = keys.Value<string>("auth");

                Subscriber sub = new Subscriber
                {
                    auth = auth,
                    p256dh = p256dh,
                    pushEndpoint = endpoint
                };

                db.Subscribers.Add(sub);
                db.SaveChanges();
            }
            return Redirect("Home");
        }
    }
}
