using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERASignup.App_Code
{
    public class NewWebhook_Data
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string topic { get; set; }
        public string resource { get; set; }
        public string _event { get; set; }
        public string[] hooks { get; set; }
        public string delivery_url { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_created_gmt { get; set; }
        public object date_modified { get; set; }
        public object date_modified_gmt { get; set; }
        public _Links _links { get; set; }
    }

    public class _Links
    {
        public Self[] self { get; set; }
        public Collection[] collection { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Collection
    {
        public string href { get; set; }
    }

}