using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERASignup.App_Code
{
    public class wpUltimo_Data_SiteDeleted
    {
        public string GetAllProps()
        {
            string props = null;
            props = string.Concat("user_id", ": ", data.user_id);
            props += string.Concat(", user_email", ": ", data.user_email);
            props += string.Concat(", user_name", ": ", data.user_name);
            props += string.Concat(", date", ": ", data.date);

            return props;
        }
        public int webhook_id { get; set; }
        public string type { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public int user_id { get; set; }
            public string user_name { get; set; }
            public string user_email { get; set; }
            public string date { get; set; }
        }

    }
}