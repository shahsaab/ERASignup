using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERASignup.App_Code
{
    public class wpUltimo_WebHook_Data
    {
        public string GetAllProps()
        {
            string props = null;
            props = string.Concat("webhook_id", ": ", webhook_id);
            props += string.Concat("type", ": ", type);
            props += string.Concat("user_id", ": ", data.user_id);
            props += string.Concat(", user_site_id", ": ", data.user_site_id);
            props += string.Concat(", plan_id", ": ", data.plan_id);
            props += string.Concat(", trial_days", ": ", data.trial_days);
            props += string.Concat(", billing_frequency", ": ", data.billing_frequency);
            props += string.Concat(", template_id", ": ", data.template_id);
            props += string.Concat(", price", ": ", data.price);
            props += string.Concat(", user_login", ": ", data.user_login);
            props += string.Concat(", user_firstname", ": ", data.user_firstname);
            props += string.Concat(", user_lastname", ": ", data.user_lastname);
            props += string.Concat(", user_email", ": ", data.user_email);
            props += string.Concat(", user_site_url", ": ", data.user_site_url);
            props += string.Concat(", plan_name", ": ", data.plan_name);
            props += string.Concat(", created_at", ": ", data.created_at);
            props += string.Concat(", user_site_name", ": ", data.user_site_name);
            props += string.Concat(", user_site_slug", ": ", data.user_site_slug);

            return props;
        }
        public int webhook_id { get; set; }
        public string type { get; set; }
        public dataClass data { get; set; }
        //        {
        //  "webhook_id": 57,
        //  "type": "account_created",
        //  "data": {
        //    "user_id": 1,
        //    "user_site_id": 1,
        //    "plan_id": 1,
        //    "trial_days": 10,
        //    "billing_frequency": 12,
        //    "template_id": 1,
        //    "price": 9.99,
        //    "user_login": "johndoe",
        //    "user_firstname": "John",
        //    "user_lastname": "Doe",
        //    "user_email": "johndoe@acme.com",
        //    "user_site_url": "http://test.mynetwork.com/",
        //    "plan_name": "Medium",
        //    "created_at": "2018-06-30 10:23:25",
        //    "user_site_name": "John's Blog",
        //    "user_site_slug": "test"
        //  }
        //}
    }
    public class dataClass
    {
        public int user_id { get; set; }
        public int user_site_id { get; set; }
        public int plan_id { get; set; }
        public int trial_days { get; set; }
        public int billing_frequency { get; set; }
        public string template_id { get; set; }
        public float price { get; set; }
        public string user_login { get; set; }
        public string user_firstname { get; set; }
        public string user_lastname { get; set; }
        public string user_email { get; set; }
        public string user_site_url { get; set; }
        public string plan_name { get; set; }
        public DateTime created_at { get; set; }
        public string user_site_name { get; set; }
        public string user_site_slug { get; set; }
    }
}