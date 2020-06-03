using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERASignup.App_Code
{

    public class NewAPIKey_Data
    {
        public int key_id { get; set; }
        public int user_id { get; set; }
        public string consumer_key { get; set; }
        public string consumer_secret { get; set; }
        public string key_permissions { get; set; }
    }
}