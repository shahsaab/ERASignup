using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERASignup.ClassTypes.Shipping
{

    public class ShippingZone
    {
        public int id { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public _Links _links { get; set; }
    }

    public class _Links
    {
        public Self[] self { get; set; }
        public Collection[] collection { get; set; }
        public Describedby[] describedby { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Collection
    {
        public string href { get; set; }
    }

    public class Describedby
    {
        public string href { get; set; }
    }


    public class ShippingZoneLocation
    {
        public string code { get; set; }
        public string type { get; set; }
        public _Links2 _links { get; set; }
    }

    public class _Links2
    {
        public Collection2[] collection { get; set; }
        public Describe[] describes { get; set; }
    }

    public class Collection2
    {
        public string href { get; set; }
    }

    public class Describe
    {
        public string href { get; set; }
    }


}
