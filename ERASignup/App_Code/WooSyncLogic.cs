using ERASignup.ClassTypes.ShippingMethod;
using System;
using System.Data;
using System.Linq;

namespace ERASignup.App_Code
{
    public class WooSyncLogic
    {
        public string error { get; set; }
        public bool SyncProducts(string DBName, string APIKey, string APISecret, string APIURL)
        {
            try
            {
                SetSyncLog(DBName, "SyncProducts Process Started...", true);

                DAL db = new DAL(DBName);

                DataTable dtProducts = db.execQuery("Get_ProductListForECommerce", System.Data.CommandType.Text, null);
                DataTable dtCategories = db.execQuery("select distinct Category from Products", System.Data.CommandType.Text, null);

                SetSyncLog(DBName, "Products Found (EraConnect): " + dtProducts.Rows.Count, true);
                SetSyncLog(DBName, "Categories Found (EraConnect): " + dtCategories.Rows.Count, true);


                if (dtProducts != null && dtProducts.Rows.Count > 0)
                {
                    WooWrapper woo = new WooWrapper(APIKey, APISecret, APIURL);
                    ClassTypes.Products.Product[] AllProducts = woo.GetAllProducts();
                    ClassTypes.Categories.Category[] AllCategories = woo.GetAllCategories();

                    SetSyncLog(DBName, "Products Found (EraLive): " + AllProducts.Length, true);
                    SetSyncLog(DBName, "Categories Found (EraLive): " + AllCategories.Length, true);

                    foreach (ClassTypes.Categories.Category cat in AllCategories)
                    {
                        if (cat.name == "Uncategorized")
                            continue;

                        bool exists = dtCategories.Select("Category = '" + cat.name + "'").Length > 0;
                        if (exists)
                            continue;

                        if (woo.DeleteCategory(cat.id.Value) == null)
                            SetSyncLog(DBName, "Error while deleting Category (EraLive): " + cat.name + ". Error: " + woo.error, true);

                    }

                    foreach (DataRow row in dtCategories.Rows)
                    {
                        string CategoryName = row["Category"].ToString();
                        bool exists = AllCategories.Any(x => x.name == CategoryName);
                        if (!exists)
                        {
                            ClassTypes.Categories.Category newCategory = new ClassTypes.Categories.Category()
                            {
                                name = CategoryName,
                                display = "default"
                            };
                            if (woo.AddCategory(newCategory) == null)
                                SetSyncLog(DBName, "Error while adding Category (EraLive): " + newCategory.name + ". Error: " + woo.error, true);
                        }
                    }

                    AllCategories = woo.GetAllCategories();

                    foreach (ClassTypes.Products.Product prod in AllProducts)
                    {
                        bool exists = dtProducts.Select("Barcode = '" + prod.sku + "'").Length > 0;
                        if (!exists)
                            if (woo.DeleteProduct(prod.id.Value) == null)
                                SetSyncLog(DBName, "Error while Deleting Product (EraLive): " + prod.name + ". Error: " + woo.error, true);
                    }

                    foreach (DataRow row in dtProducts.Rows)
                    {
                        string ProductBarcode = row["barcode"].ToString();
                        bool exists = AllProducts.Any(x => x.sku == ProductBarcode);
                        if (!exists)
                        {
                            ClassTypes.Products.Category[] c = new ClassTypes.Products.Category[1];
                            c[0] = new ClassTypes.Products.Category();
                            var MatchedCat = AllCategories.Where(ca => ca.name.ToLower() == Helper.HtmlEncode(row["Category"].ToString().ToLower())).FirstOrDefault();

                            if (MatchedCat != null)
                                c[0].id = MatchedCat.id.Value;


                            ClassTypes.Products.Image[] images = null;
                            //iterate trhough images and add in the above object
                            if (row["Images"].ToString() != string.Empty)
                            {
                                string ImageStoreURL = System.Configuration.ConfigurationManager.AppSettings["ImageStore"];
                                string[] imgURLs = row["Images"].ToString().Substring(0, row["Images"].ToString().Length - 2).Split(',');

                                images = new ClassTypes.Products.Image[imgURLs.Length];

                                for (int i = 0; i < imgURLs.Length; i++)
                                {
                                    images[i] = new ClassTypes.Products.Image();
                                    images[i].src = string.Concat(ImageStoreURL, imgURLs[i].Trim());
                                    images[i].alt = row["Name"].ToString();
                                }
                            }

                            ClassTypes.Products.Product newProd = new ClassTypes.Products.Product()
                            {
                                sku = ProductBarcode,
                                name = row["Name"].ToString(),
                                categories = c,
                                short_description = row["Description"].ToString(),
                                regular_price = row["RetailPrice"].ToString(),
                                status = row["Status"].ToString() == "True" ? "publish" : "pending",
                                stock_status = "instock",
                                catalog_visibility = "visible",
                                sale_price = row["SalePrice"].ToString(),
                                date_on_sale_from_gmt = row["PromotionStart"].ToString(),
                                date_on_sale_to_gmt = row["PromotionEnd"].ToString(),
                                images = images
                            };
                            if (woo.AddProduct(newProd) == null)
                                SetSyncLog(DBName, "Error while adding Product (EraLive): " + newProd.name + ". Error: " + woo.error, true);

                        }
                        else
                        {
                            DateTime ProductModifiedAt = DateTime.Parse(row["ModifiedAt"].ToString());
                            ClassTypes.Products.Product prod = AllProducts.Where(x => x.sku == ProductBarcode).FirstOrDefault();
                            if (prod.date_modified_gmt.HasValue && ProductModifiedAt > prod.date_modified_gmt)
                            {
                                ClassTypes.Products.Category[] c = new ClassTypes.Products.Category[1];

                                c[0] = new ClassTypes.Products.Category();
                                var MatchedCat = AllCategories.Where(ca => ca.name.ToLower() == Helper.HtmlEncode(row["Category"].ToString().ToLower())).FirstOrDefault();

                                if (MatchedCat != null)
                                    c[0].id = MatchedCat.id.Value;

                                ClassTypes.Products.Image[] images = null;
                                //iterate trhough images and add in the above object
                                if (row["Images"].ToString() != string.Empty)
                                {
                                    string ImageStoreURL = System.Configuration.ConfigurationManager.AppSettings["ImageStore"];
                                    string[] imgURLs = row["Images"].ToString().Substring(0, row["Images"].ToString().Length - 2).Split(',');

                                    images = new ClassTypes.Products.Image[imgURLs.Length];

                                    for (int i = 0; i < imgURLs.Length; i++)
                                    {
                                        images[i] = new ClassTypes.Products.Image();
                                        images[i].src = string.Concat(ImageStoreURL, imgURLs[i].Trim());
                                        images[i].alt = row["Name"].ToString();
                                    }
                                }

                                prod.name = row["Name"].ToString();
                                prod.short_description = row["Description"].ToString();
                                prod.regular_price = row["RetailPrice"].ToString();
                                prod.status = row["Status"].ToString() == "True" ? "publish" : "pending";
                                prod.stock_status = "instock";
                                prod.categories = c;
                                prod.catalog_visibility = "visible";
                                prod.images = images;
                            }

                            if (woo.UpdateProduct(prod) == null)
                                SetSyncLog(DBName, "Error while updating Product (EraLive): " + prod.name + ". Error: " + woo.error, true);
                        }
                    }

                }
                SetSyncLog(DBName, "SyncProducts process completed!", true);
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = "Error: " + ex.Message;
                if (ex.InnerException != null && ex.InnerException.Message != string.Empty)
                    errorMessage += "| Inner Exception: " + ex.InnerException.Message;

                SetSyncLog(DBName, "Error while Products & Categories Sync. " + errorMessage, true);
                error = ex.Message;
                return false;
            }
        }

        public bool SyncOrders(string DBName, string APIKey, string APISecret, string APIURL, DateTime LastSyncedAt)
        {
            try
            {
                SetSyncLog(DBName, "SyncOrders Process Started...", true);

                DAL db = new DAL(DBName);
                DataTable dtOrders = db.execQuery("select ID, Status from eOrder where ModifiedAt >= '" + LastSyncedAt + "' and Status in ('Delivered','Cancelled')", System.Data.CommandType.Text, null);

                SetSyncLog(DBName, "Orders Found (EraConnect): " + dtOrders.Rows.Count, true);

                if (dtOrders != null && dtOrders.Rows.Count > 0)
                {
                    WooWrapper woo = new WooWrapper(APIKey, APISecret, APIURL);
                    foreach (DataRow row in dtOrders.Rows)
                    {
                        string DBStatus = row["Status"].ToString();
                        ClassTypes.Orders.Order ord = new ClassTypes.Orders.Order();
                        ord.status = DBStatus == "Delivered" ? "completed" : "cancelled";
                        if(woo.UpdateOrder(ord) == null)
                            SetSyncLog(DBName, "Error while updating order (EraLive): " + ord.id + ". Error: " + woo.error, true);
                    }
                }
                SetSyncLog(DBName, "SyncOrders Process Completed!", true);
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = "Error: " + ex.Message;
                if (ex.InnerException != null && ex.InnerException.Message != string.Empty)
                    errorMessage += "| Inner Exception: " + ex.InnerException.Message;

                SetSyncLog(DBName, "Error while Order Sync. " + errorMessage, true);

                error = ex.Message;
                return false;
            }
        }

        public bool DeleteAllProducts(string SubDomain, string APIKey, string APISecret, string APIURL)
        {
            try
            {
                SetSyncLog(SubDomain, "Deleting Products & Categories (EraLive). Process Started...", true);

                WooWrapper woo = new WooWrapper(APIKey, APISecret, APIURL);
                ClassTypes.Products.Product[] AllProducts = woo.GetAllProducts();
                ClassTypes.Categories.Category[] AllCategories = woo.GetAllCategories();

                SetSyncLog(SubDomain, "Products Found (EraLive): " + AllProducts.Length, true);
                SetSyncLog(SubDomain, "Categories Found (EraLive): " + AllCategories.Length, true);

                foreach (ClassTypes.Categories.Category cat in AllCategories)
                {
                    if (cat.name == "Uncategorized")
                        continue;

                    if (woo.DeleteCategory(cat.id.Value) == null)
                        SetSyncLog(SubDomain, "Error while deleting category (EraLive): " + cat.name + ". Error: " + woo.error, true);
                }

                foreach (ClassTypes.Products.Product prod in AllProducts)
                {
                    if (woo.DeleteProduct(prod.id.Value) == null)
                        SetSyncLog(SubDomain, "Error while deleting product (EraLive): " + prod.name + ". Error: " + woo.error, true);
                }

                SetSyncLog(SubDomain, "Products & categories deleted (EraLive)...", true);

                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = "Error: " + ex.Message;
                if (ex.InnerException != null && ex.InnerException.Message != string.Empty)
                    errorMessage += "| Inner Exception: " + ex.InnerException.Message;

                SetSyncLog(SubDomain, "Error while deleting products & categories (EraLive). " + errorMessage, true);

                error = ex.Message;
                return false;
            }
        }

        public bool SyncDelivery_Shipping(string DBName, string APIKey, string APISecret, string APIURL, DateTime LastSyncedAt)
        {
            try
            {
                SetSyncLog(DBName, "SyncDeliver_Shipping Process Started...", true);

                DAL db = new DAL(DBName);
                DataTable dtShipping = db.execQuery("select * from eDelivery_Shipping where ModifiedAt >'" + LastSyncedAt + "'", System.Data.CommandType.Text, null);
                SetSyncLog(DBName, "eDeliver_Shipping Found: " + dtShipping.Rows.Count, true);

                if (dtShipping != null && dtShipping.Rows.Count > 0)
                {
                    WooWrapper woo = new WooWrapper(APIKey, APISecret, APIURL);
                    ClassTypes.Shipping.ShippingZone[] AllZones = woo.GetAllShippingZones();

                    foreach (DataRow row in dtShipping.Rows)
                    {
                        var zone = AllZones.Where(z => z.name == row["Type"].ToString()).FirstOrDefault();
                        if (zone == null)
                            continue;

                        if (!string.IsNullOrEmpty(row["Zone"].ToString()))
                        {
                            string jsonStr = "[{\"code\": \"<<zone>>\",\"type\": \"state\"}]";

                            jsonStr = jsonStr.Replace("<<zone>>", "PK:" + row["Zone"].ToString());

                            if(!woo.UpdateShippingZoneLocation(jsonStr, zone.id))
                                SetSyncLog(DBName, "Error while Updating Shipping Zone's Location (EraLive): " + zone.id + ". Error: " + woo.error, true);
                        }

                        ShippingMethod[] Methods = woo.GetAllShippingZoneMethods(zone.id);

                        if (Methods == null)
                            continue;

                        string jsonString = "{\"id\":\"<<id>>\", \"enabled\":<<enabled>>,\"settings\": {\"cost\": \"<<cost>>\", \"min_amount\":\"<<min_amount>>\"}}";
                        string Charges = row["Charges"].ToString();
                        string Min_Amount = row["MinimumOrderAmount"].ToString();
                        bool Enabled = row["Status"].ToString() == "True";

                        jsonString = jsonString.Replace("<<id>>", Methods[0].id.ToString());
                        jsonString = jsonString.Replace("<<enabled>>", Enabled.ToString().ToLower());
                        jsonString = jsonString.Replace("<<cost>>", Charges);

                        //If Minimum Amount is not null or empty        min_amount field will be removed from json String         else  Min_Amount value is set
                        jsonString = string.IsNullOrEmpty(Min_Amount) ? jsonString.Replace(", \"min_amount\":\"<<min_amount>>\"", "") : jsonString.Replace("<<min_amount>>", Min_Amount);

                        if(woo.UpdateShippingZoneMethod(jsonString, Methods[0].id, zone.id) == null)
                            SetSyncLog(DBName, "Error while Updating Shipping Zone Method (EraLive): " + zone.id + ". Error: " + woo.error, true);
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
                string errorMessage = "Error: " + ex.Message;
                if (ex.InnerException != null && ex.InnerException.Message != string.Empty)
                    errorMessage += "| Inner Exception: " + ex.InnerException.Message;

                SetSyncLog(DBName, "Error while Delivery/Shipping Sync. " + errorMessage, true);

                error = ex.Message;
                return false;
            }
        }

        public void SetSyncLog(string SubDomain, string Message, bool isSuccess)
        {
            DAL db = new DAL("Accounts");
            System.Data.SqlClient.SqlParameter[] para =
            {
                new System.Data.SqlClient.SqlParameter("@SubDomain", SubDomain),
                new System.Data.SqlClient.SqlParameter("@Message", Message),
                new System.Data.SqlClient.SqlParameter("@Success", isSuccess)
            };
            db.execQuery("Set_SyncLogs", CommandType.StoredProcedure, para);
        }
    }
}