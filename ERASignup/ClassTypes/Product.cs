using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERASignup.ClassTypes.Products
{
    public class Product
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string permalink { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_created_gmt { get; set; }
        public DateTime? date_modified { get; set; }
        public DateTime? date_modified_gmt { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public bool featured { get; set; }
        public string catalog_visibility { get; set; }
        public string description { get; set; }
        public string short_description { get; set; }
        public string sku { get; set; }
        public string price { get; set; }
        public string regular_price { get; set; }
        public string sale_price { get; set; }
        public object date_on_sale_from { get; set; }
        public object date_on_sale_from_gmt { get; set; }
        public object date_on_sale_to { get; set; }
        public object date_on_sale_to_gmt { get; set; }
        public string price_html { get; set; }
        public bool? on_sale { get; set; }
        public bool? purchasable { get; set; }
        public int? total_sales { get; set; }
        public bool? _virtual { get; set; }
        public bool? downloadable { get; set; }
        public object[] downloads { get; set; }
        public int? download_limit { get; set; }
        public int? download_expiry { get; set; }
        public string external_url { get; set; }
        public string button_text { get; set; }
        public string tax_status { get; set; }
        public string tax_class { get; set; }
        public bool? manage_stock { get; set; }
        public object stock_quantity { get; set; }
        public string stock_status { get; set; }
        public string backorders { get; set; }
        public bool? backorders_allowed { get; set; }
        public bool? backordered { get; set; }
        public object sold_individually { get; set; }
        public string weight { get; set; }
        public Dimensions dimensions { get; set; }
        public bool? shipping_required { get; set; }
        public bool? shipping_taxable { get; set; }
        public string shipping_class { get; set; }
        public int? shipping_class_id { get; set; }
        public bool reviews_allowed { get; set; }
        public string average_rating { get; set; }
        public int? rating_count { get; set; }
        public int?[] related_ids { get; set; }
        public object[] upsell_ids { get; set; }
        public object[] cross_sell_ids { get; set; }
        public int? parent_id { get; set; }
        public string purchase_note { get; set; }
        public Category[] categories { get; set; }
        public object[] tags { get; set; }
        public object[] images { get; set; }
        public object[] attributes { get; set; }
        public object[] default_attributes { get; set; }
        public object[] variations { get; set; }
        public object[] grouped_products { get; set; }
        public int? menu_order { get; set; }
        public object[] meta_data { get; set; }
        public _Links _links { get; set; }

        enum Type
        { simple, grouped, external, variable }

        enum Status
        { draft, pending, publish } //There is another status = private,

        enum Catalog_Visibility
        { visible, catalog, search, hidden }

        enum Tax_Status
        { taxable, shipping, none }

        enum Stock_Status
        { instock, outofstock, onbackorder }
    }

    public class Dimensions
    {
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
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

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
    }

    public class Image
    {
        public string src { get; set; }
        public string name { get; set; }
        public string alt { get; set; }
    }
}
