using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERASignup.ClassTypes.ShippingMethod
{
    public class ShippingMethod
    {
        public int id { get; set; }
        public int instance_id { get; set; }
        public string title { get; set; }
        public int order { get; set; }
        public bool enabled { get; set; }
        public string method_id { get; set; }
        public string method_title { get; set; }
        public string method_description { get; set; }
        public Settings settings { get; set; }
        public _Links _links { get; set; }
    }

    public class Settings
    {
        public Title title { get; set; }
        public Tax_Status tax_status { get; set; }
        public Cost cost { get; set; }
        public Min_Amount min_amount { get; set; }
    }

    public class Title
    {
        public string id { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string _default { get; set; }
        public string tip { get; set; }
        public string placeholder { get; set; }
    }

    public class Tax_Status
    {
        public string id { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string _default { get; set; }
        public string tip { get; set; }
        public string placeholder { get; set; }
        public Options options { get; set; }
    }

    public class Options
    {
        public string taxable { get; set; }
        public string none { get; set; }
    }

    public class Cost
    {
        public string id { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string _default { get; set; }
        public string tip { get; set; }
        public string placeholder { get; set; }
    }


    public class Rootobject
    {
        public Min_Amount min_amount { get; set; }
    }

    public class Min_Amount
    {
        public string id { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string _default { get; set; }
        public string tip { get; set; }
        public string placeholder { get; set; }
    }


    public class _Links
    {
        public Self[] self { get; set; }
        public Collection[] collection { get; set; }
        public Describe[] describes { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Collection
    {
        public string href { get; set; }
    }

    public class Describe
    {
        public string href { get; set; }
    }
}
