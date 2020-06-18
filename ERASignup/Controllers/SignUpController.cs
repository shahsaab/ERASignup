using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

namespace ERASignup.Controllers
{
    public class SignUpController : ApiController
    {
        string Key = "ck_497bb11426365344b77ab62a070bdf75f75b55d5";// <-- Alkhait "ck_1a924ab52e7089bcd9021072796183455087f416";
        string Secret = "cs_747cfb3d3c58a1b5619e1a4c6b2b259a852095bc"; // <-- AlKhait "cs_dbb4e06bf715bc0ead6e928ca5da4ecfd90f3682";

        [HttpPost]
        [ActionName("NewSite")]
        public string NewSite([FromBody] App_Code.wpUltimo_WebHook_Data data)
        {
            DAL db = new DAL("Accounts");
            SqlParameter[] para =
            {
                new SqlParameter("@log", data.GetAllProps()),
                new SqlParameter("@controllerName", "SignUp"),
                new SqlParameter("@action", "NewSite")
            };
            db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para);

            SqlParameter[] para2 =
            {
              new SqlParameter("@SubDomain", data.data.user_site_slug),
              new SqlParameter("@SiteID", data.data.user_site_id),
              new SqlParameter("@SiteName", data.data.user_site_name),
              new SqlParameter("@SiteURL", data.data.user_site_url),
              new SqlParameter("@UserID", data.data.user_id),
              new SqlParameter("@UserFirstName", data.data.user_firstname),
              new SqlParameter("@UserLastName", data.data.user_lastname),
              new SqlParameter("@UserEmail", data.data.user_email),
              new SqlParameter("@UserLogin", data.data.user_login),
              new SqlParameter("@CreatedAt", data.data.created_at),
              new SqlParameter("@PlanID", data.data.plan_id),
              new SqlParameter("@PlanName", data.data.plan_name)
            };

            db.execQuery("Set_UserAccount", CommandType.StoredProcedure, para2);
            DAL db2 = new DAL("Master");
            db2.execQuery("CopyDB '" + data.data.user_site_slug + "'", CommandType.Text, null);

            if (db.ExceptionMsg == null && db2.ExceptionMsg == null)
            {
                //CallAPI(data.data.user_site_url, Method.POST, null, null);
                CreateWebHook(data.data.user_site_url, data.data.user_site_slug);
                SetCredentials(data.data.user_site_slug, data.data.user_login);

                return "Yeah!";
            }
            else
            {
                SqlParameter[] para3 =
                {
                    new SqlParameter("@log", db.ExceptionMsg),
                    new SqlParameter("@controllerName", "SignUp"),
                    new SqlParameter("@action", "NewSite")
                };
                db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para3);
                return db.ExceptionMsg;

            }
        }

        [HttpPost]
        [ActionName("NewAPIKey")]
        public void NewAPIKey([FromBody] App_Code.NewAPIKey_Data data)
        {
            DAL db = new DAL("Accounts");
            SqlParameter[] para3 =
                {
                    new SqlParameter("@log", JsonConvert.SerializeObject(data)),
                    new SqlParameter("@controllerName", "SignUp"),
                    new SqlParameter("@action", "NewAPIKey")
                };
            db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para3);
        }

        [HttpGet]
        [ActionName("CreateNewAPIKey")]
        public string CreateNewAPIKey(string WebSite)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("app_name", "EraConnect");
            parameters.Add("user_id", "995");
            parameters.Add("scope", "read_write");
            parameters.Add("return_url", "https://eralive.net");
            parameters.Add("callback_url", "https://eraconnect.net/signup/api/signup/NewAPIKey/");

            string apipath = string.Concat(WebSite, "/wc-auth/v1/authorize");
            string response = CallAPI(apipath, Method.POST, false, null, parameters);

            DAL db = new DAL("Accounts");
            SqlParameter[] para3 =
                {
                    new SqlParameter("@log", response),
                    new SqlParameter("@controllerName", "SignUp"),
                    new SqlParameter("@action", "NewAPIKey")
                };
            db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para3);
            return response;

        }

        public bool CreateWebHook(string WebSite, string slug)
        {
            string apiPath = string.Concat(WebSite, "/wp-json/wc/v3/webhooks");
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("consumer_key", Key);
            Parameters.Add("consumer_secret", Secret);
            Parameters.Add("name", "OrderToEra");
            Parameters.Add("topic", "order.created");
            Parameters.Add("delivery_url", "https://"+ slug +".eraconnect.net/ERAAPI/api/Woo/OrderCreated");

            string response = CallAPI(apiPath, Method.POST,true, null, Parameters);

            if (response.Contains("Error"))
                return false;
            else
                return true;
        }

        public string CallAPI(string apiPath, Method method, bool GotKey = true, object bodyParameter = null, Dictionary<string, string> Parameters = null)
        {
            var client = new RestClient(apiPath);

            var request = new RestRequest(method);
            if (GotKey)
            {
                request.AddQueryParameter("consumer_key", Key);
                request.AddQueryParameter("consumer_secret", Secret);
            }
            else if (Parameters != null)
                foreach(KeyValuePair<string ,string> para in Parameters)
                    request.AddQueryParameter(para.Key, para.Value);

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                return response.Content;
            else
                return "Error: " + response.Content;
        }

        public void SetCredentials(string slug, string Username)
        {
            DAL db = new DAL(slug);
            db.execQuery("update users set user_name='" + Username + "', Password='" + Username + "', isFirstLogin=1", CommandType.Text, null);
        }
    }
}
