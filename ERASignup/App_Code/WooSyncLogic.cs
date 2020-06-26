using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERASignup.App_Code
{
    public class WooSyncLogic
    {
        public string error { get; set; }
        public bool SyncProducts(string DBName, string APIKey, string APISecret, string APIURL)
        {
            try
            {
                DAL db = new DAL(DBName);
                
                DataTable dtProducts = db.execQuery("Get_ProductListForECommerce", System.Data.CommandType.Text, null);
                DataTable dtCategories = db.execQuery("select distinct Category from Products", System.Data.CommandType.Text, null);

                if (dtProducts != null && dtProducts.Rows.Count > 0)
                {
                    WooWrapper woo = new WooWrapper(APIKey, APISecret, APIURL);
                    ClassTypes.Products.Product[] AllProducts = woo.GetAllProducts();
                    ClassTypes.Categories.Category[] AllCategories = woo.GetAllCategories();

                    foreach (ClassTypes.Categories.Category cat in AllCategories)
                    {
                        if (cat.name == "Uncategorized")
                            continue;

                        bool exists = dtCategories.Select("Category = '" + cat.name + "'").Length > 0;
                        if (!exists)
                            woo.DeleteCategory(cat.id.Value);
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
                            woo.AddCategory(newCategory);
                        }
                    }

                    AllCategories = woo.GetAllCategories();

                    foreach (ClassTypes.Products.Product prod in AllProducts)
                    {
                        bool exists = dtProducts.Select("Barcode = '" + prod.sku + "'").Length > 0;
                        if (!exists)
                            woo.DeleteProduct(prod.id.Value);
                    }

                    foreach (DataRow row in dtProducts.Rows)
                    {
                        string ProductBarcode = row["barcode"].ToString();
                        bool exists = AllProducts.Any(x => x.sku == ProductBarcode);
                        if (!exists)
                        {
                            ClassTypes.Products.Category[] c = new ClassTypes.Products.Category[1];
                            c[0] = new ClassTypes.Products.Category()
                            {
                                id = AllCategories.Where(ca => ca.name == Helper.HtmlEncode(row["Category"].ToString())).FirstOrDefault().id.Value
                            };

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
                            woo.AddProduct(newProd);
                        }
                        else
                        {
                            DateTime ProductModifiedAt = DateTime.Parse(row["ModifiedAt"].ToString());
                            ClassTypes.Products.Product prod = AllProducts.Where(x => x.sku == ProductBarcode).FirstOrDefault();
                            if (prod.date_modified_gmt.HasValue && ProductModifiedAt > prod.date_modified_gmt)
                            {
                                ClassTypes.Products.Category[] c = new ClassTypes.Products.Category[1];
                                c[0] = new ClassTypes.Products.Category()
                                {
                                    id = AllCategories.Where(ca => ca.name == row["Category"].ToString()).FirstOrDefault().id.Value
                                };

                                prod.name = row["Name"].ToString();
                                prod.short_description = row["Description"].ToString();
                                prod.regular_price = row["RetailPrice"].ToString();
                                prod.status = row["Status"].ToString() == "True" ? "publish" : "pending";
                                prod.stock_status = "instock";
                                prod.categories = c;
                                prod.catalog_visibility = "visible";
                            }

                            woo.UpdateProduct(prod);
                        }
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool SyncOrders(string DBName, string APIKey, string APISecret, string APIURL, DateTime LastSyncedAt)
        {
            try
            {
                DAL db = new DAL(DBName);
                DataTable dtOrders = db.execQuery("select ID, Status from eOrder where ModifiedAt >= '" + LastSyncedAt + "' and Status in ('Delivered','Cancelled')", System.Data.CommandType.Text, null);

                if (dtOrders != null && dtOrders.Rows.Count > 0)
                {
                    WooWrapper woo = new WooWrapper(APIKey, APISecret, APIURL);
                    foreach (DataRow row in dtOrders.Rows)
                    {
                        string DBStatus = row["Status"].ToString();
                        ClassTypes.Orders.Order ord = new ClassTypes.Orders.Order();
                        ord.status = DBStatus == "Delivered" ? "completed" : "cancelled";
                        woo.UpdateOrder(ord);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool DeleteAllProducts(string APIKey, string APISecret, string APIURL)
        {
            try
            {
                WooWrapper woo = new WooWrapper(APIKey, APISecret, APIURL);
                ClassTypes.Products.Product[] AllProducts = woo.GetAllProducts();
                ClassTypes.Categories.Category[] AllCategories = woo.GetAllCategories();

                foreach (ClassTypes.Categories.Category cat in AllCategories)
                {
                    if (cat.name == "Uncategorized")
                        continue;

                    woo.DeleteCategory(cat.id.Value);
                }

                foreach (ClassTypes.Products.Product prod in AllProducts)
                    woo.DeleteProduct(prod.id.Value);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool SyncDelivery_Shipping(string DBName, string APIKey, string APISecret, string APIURL, DateTime LastSyncedAt)
        {
            try
            {
                DAL db = new DAL(DBName);
                DataTable dtShipping = db.execQuery("select * from eDelivery_Shipping where ModifiedAt >'" + LastSyncedAt + "'", System.Data.CommandType.Text, null);

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
                            ClassTypes.Shipping.ShippingZoneLocation loc = new ClassTypes.Shipping.ShippingZoneLocation();
                            loc.code = "PK:" + row["Zone"].ToString();
                            loc.type = "state";
                            woo.UpdateShippingZoneLocation(loc, zone.id);
                        }

                        ClassTypes.ShippingMethod.ShippingMethod[] Methods = woo.GetAllShippingZoneMethods(zone.id);

                        if (Methods == null)
                            continue;

                        Methods[0].enabled = row["Status"].ToString() == "1";
                        Methods[0].settings.cost.value = row["Charges"].ToString();
                        if (!string.IsNullOrEmpty(row["MinimumOrderAmount"].ToString()))
                            Methods[0].settings.min_amount.value = row["MinimumOrderAmount"].ToString();

                        woo.UpdateShippingZoneMethod(Methods[0], zone.id);
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
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