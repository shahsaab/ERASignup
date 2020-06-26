using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERASignup.ClassTypes.Categories
{
    public class Category
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int? parent { get; set; }
        public string description { get; set; }
        public string display { get; set; }
        public Image image { get; set; }
        public int? menu_order { get; set; }
        public int? count { get; set; }
        public _Links _links { get; set; }

        enum Display
        { Default, products, subcategories, both }

        enum Status
        { pending, processing, completed, cancelled, refunded, failed, trash }

    }

    public class Image
    {
        public int? id { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_created_gmt { get; set; }
        public DateTime? date_modified { get; set; }
        public DateTime? date_modified_gmt { get; set; }
        public string src { get; set; }
        public string name { get; set; }
        public string alt { get; set; }
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
