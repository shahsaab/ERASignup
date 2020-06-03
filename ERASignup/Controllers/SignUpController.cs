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
        [HttpPost]
        [ActionName("NewSite")]
        public string NewSite([FromBody] App_Code.wpUltimo_WebHook_Data data)
        {
            DAL db = new DAL();
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
            DAL db2 = new DAL("CS2");
            db2.execQuery("CopyDB '" + data.data.user_site_slug + "'", CommandType.Text, null);

            if (db.ExceptionMsg == null && db2.ExceptionMsg == null)
            {
                //CallAPI(data.data.user_site_url, Method.POST, null, null);
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
            DAL db = new DAL("CS");
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
            string response = CallAPI(WebSite, Method.POST, null, null);
            DAL db = new DAL("CS");
            SqlParameter[] para3 =
                {
                    new SqlParameter("@log", response),
                    new SqlParameter("@controllerName", "SignUp"),
                    new SqlParameter("@action", "NewAPIKey")
                };
            db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para3);
            return response;

        }

        public string CallAPI(string apiPath, Method method, object bodyParameter = null, Dictionary<string, string> Parameters = null)
        {
            string endpoint = "/wc-auth/v1/authorize";
            var client = new RestClient(string.Concat(apiPath, endpoint));

            var request = new RestRequest(method);
            request.AddQueryParameter("app_name", "EraConnect");
            request.AddQueryParameter("user_id", "1");
            request.AddQueryParameter("scope", "read_write");
            request.AddQueryParameter("return_url", "https://eralive.net");
            request.AddQueryParameter("callback_url", "https://eraconnect.net/signup/api/signup/NewAPIKey/");

            //if (Parameters != null)
            //    foreach (KeyValuePair<string, string> para in Parameters)
            //        request.AddParameter(para.Key, para.Value);

            //if (bodyParameter != null)
            //    request.AddJsonBody(bodyParameter);

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                return response.Content;
            else
                return "Error: " + response.Content;
        }
    }
}
