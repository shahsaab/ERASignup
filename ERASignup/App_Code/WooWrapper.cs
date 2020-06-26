using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace ERASignup.App_Code
{
    public class WooWrapper
    {
        string Key, Secret, apiURL;

        public WooWrapper(string APIKey, string APISecret, string APIURL)
        {
            Key = APIKey;
            Secret = APISecret;
            apiURL = APIURL;
        }

        public string CallAPI(string apiPath, Method method, object bodyParameter = null, Dictionary<string, string> Parameters = null)
        {
            var client = new RestClient(string.Concat(apiURL, apiPath));

            var request = new RestRequest(method);
            request.AddQueryParameter("consumer_key", Key);
            request.AddQueryParameter("consumer_secret", Secret);
            request.AddQueryParameter("per_page", "50");

            if (Parameters != null)
                foreach (KeyValuePair<string, string> para in Parameters)
                    request.AddParameter(para.Key, para.Value);

            if (bodyParameter != null)
                request.AddJsonBody(bodyParameter);

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                return response.Content;
            else
                return "Error: " + response.Content;
        }

        #region Products
        public ClassTypes.Products.Product[] GetAllProducts()
        {
            string response = CallAPI("products", Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var prod = JsonConvert.DeserializeObject<ClassTypes.Products.Product[]>(response);
            return prod;

        }

        public ClassTypes.Products.Product GetProduct(string ProductId)
        {
            string response = CallAPI(string.Concat("products", "/", ProductId), Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var prod = JsonConvert.DeserializeObject<ClassTypes.Products.Product>(response);
            return prod;
        }

        public ClassTypes.Products.Product AddProduct(ClassTypes.Products.Product newProduct)
        {
            string bodyPara = JsonConvert.SerializeObject(newProduct, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string response = CallAPI("products/", Method.POST, bodyPara);
            if (response.Contains("Error"))
                return null;

            var prod = JsonConvert.DeserializeObject<ClassTypes.Products.Product>(response);
            return prod;
        }

        public ClassTypes.Products.Product UpdateProduct(ClassTypes.Products.Product product)
        {
            string bodyPara = JsonConvert.SerializeObject(product, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            string response = CallAPI(string.Concat("products/", product.id), Method.PUT, bodyPara);
            if (response.Contains("Error"))
                return null;

            var prod = JsonConvert.DeserializeObject<ClassTypes.Products.Product>(response);
            return prod;
        }

        public ClassTypes.Products.Product DeleteProduct(int ProductID)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("force", "true");
            string response = CallAPI(string.Concat("products/", ProductID), Method.DELETE, Parameters);
            if (response.Contains("Error"))
                return null;

            var prod = JsonConvert.DeserializeObject<ClassTypes.Products.Product>(response);
            return prod;
        }

        #endregion

        #region Category
        public ClassTypes.Categories.Category[] GetAllCategories()
        {
            string response = CallAPI("products/categories", Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var category = JsonConvert.DeserializeObject<ClassTypes.Categories.Category[]>(response);
            return category;
        }

        public ClassTypes.Categories.Category GetCategory(int CategoryID)
        {
            string response = CallAPI(string.Concat("products/categories", "/", CategoryID), Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var category = JsonConvert.DeserializeObject<ClassTypes.Categories.Category>(response);
            return category;
        }

        public ClassTypes.Categories.Category AddCategory(ClassTypes.Categories.Category newCategory)
        {
            string bodyPara = JsonConvert.SerializeObject(newCategory, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string response = CallAPI("products/categories", Method.POST, bodyPara);
            if (response.Contains("Error"))
                return null;

            var category = JsonConvert.DeserializeObject<ClassTypes.Categories.Category>(response);
            return category;
        }

        public ClassTypes.Categories.Category UpdateCategory(ClassTypes.Categories.Category cat)
        {

            string bodyPara = JsonConvert.SerializeObject(cat, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            string response = CallAPI(string.Concat("Products/Categories/", cat.id), Method.PUT, bodyPara);
            if (response.Contains("Error"))
                return null;

            var category = JsonConvert.DeserializeObject<ClassTypes.Categories.Category>(response);
            return category;
        }

        public ClassTypes.Categories.Category DeleteCategory(int CategoryID)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("force", "true");
            string response = CallAPI(string.Concat("products/Categories/", CategoryID), Method.DELETE, Parameters);
            if (response.Contains("Error"))
                return null;

            var prod = JsonConvert.DeserializeObject<ClassTypes.Categories.Category>(response);
            return prod;
        }

        #endregion

        #region Orders

        public ClassTypes.Orders.Order[] GetAllOrders()
        {
            string response = CallAPI("orders", Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var orders = JsonConvert.DeserializeObject<ClassTypes.Orders.Order[]>(response);
            return orders;

        }

        public ClassTypes.Orders.Order GetOrder(int OrderID)
        {
            string response = CallAPI(string.Concat("orders", "/", OrderID), Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var order = JsonConvert.DeserializeObject<ClassTypes.Orders.Order>(response);
            return order;
        }

        public ClassTypes.Orders.Order UpdateOrder(ClassTypes.Orders.Order newOrder)
        {
            string bodyPara = JsonConvert.SerializeObject(newOrder, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string response = CallAPI(string.Concat("Orders/", newOrder.id), Method.PUT, bodyPara);

            if (response.Contains("Error"))
                return null;

            var order = JsonConvert.DeserializeObject<ClassTypes.Orders.Order>(response);
            return order;
        }

        #endregion

        #region ShippingZones

        public ClassTypes.Shipping.ShippingZone[] GetAllShippingZones()
        {
            string response = CallAPI("shipping/zones", Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var zones = JsonConvert.DeserializeObject<ClassTypes.Shipping.ShippingZone[]>(response);
            return zones;

        }

        public ClassTypes.ShippingMethod.ShippingMethod[] GetAllShippingZoneMethods(int ShippingZoneID)
        {
            string response = CallAPI(string.Concat("shipping/zones/", ShippingZoneID, "/methods"), Method.GET, null);
            if (response.Contains("Error"))
                return null;

            var zones = JsonConvert.DeserializeObject<ClassTypes.ShippingMethod.ShippingMethod[]>(response);
            return zones;

        }

        public ClassTypes.Shipping.ShippingZoneLocation UpdateShippingZoneLocation(ClassTypes.Shipping.ShippingZoneLocation newLocation, int ShippingZoneID)
        {
            string bodyPara = JsonConvert.SerializeObject(newLocation, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string response = CallAPI(string.Concat("shipping/zones/", ShippingZoneID, "/locations"), Method.PUT, bodyPara);

            if (response.Contains("Error"))
                return null;

            var location = JsonConvert.DeserializeObject<ClassTypes.Shipping.ShippingZoneLocation>(response);
            return location;
        }

        public ClassTypes.ShippingMethod.ShippingMethod UpdateShippingZoneMethod(ClassTypes.ShippingMethod.ShippingMethod newMethod, int ShippingZoneID)
        {
            string bodyPara = JsonConvert.SerializeObject(newMethod, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string response = CallAPI(string.Concat("shipping/zones/", ShippingZoneID, "/methods/", newMethod.id), Method.PUT, bodyPara);

            if (response.Contains("Error"))
                return null;

            var method = JsonConvert.DeserializeObject<ClassTypes.ShippingMethod.ShippingMethod>(response);
            return method;
        }

        #endregion
    }
}