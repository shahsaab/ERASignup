using Newtonsoft.Json;
using RestSharp;
using System;
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
            SetLog("NewSite", data.GetAllProps());

            DAL db = new DAL("Accounts");

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
                SetCredentials(data.data.user_site_slug, data.data.user_login);
                return "Yeah!";
            }
            else
            {
                SetLog("NewSite", db.ExceptionMsg);
                return db.ExceptionMsg;

            }
        }

        public void SetLog(string Action, string LogMessage)
        {
            DAL db = new DAL("Accounts");
            SqlParameter[] para =
                {
                    new SqlParameter("@log", LogMessage),
                    new SqlParameter("@controllerName", "SignUp"),
                    new SqlParameter("@action", Action)
                };
            db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para);
        }

        [HttpGet]
        [ActionName("NewAPIKey")]
        public bool NewAPIKey(string SubDomain, string Key, string Secret)
        {
            DAL db = new DAL("Accounts");
            System.Data.DataTable dt = db.execQuery("select SiteURL from userAccounts where SubDomain='"+ SubDomain +"' and Status=1", System.Data.CommandType.Text, null);

            if (dt == null || dt.Rows.Count == 0)
                return false;

            string Response = SetAPIKey(dt.Rows[0]["SiteURL"].ToString(), SubDomain, Key, Secret);

            SetLog("New API Key", Response);

            return Response.Contains("Success");
        }

        public string SetAPIKey(string SiteURL, string SubDomain, string Key, string Secret)
        {
            try
            {
                //request logged
                SetLog("SetAPIKey", string.Concat("SubDomain: ", SubDomain, " - SiteURL: ", SiteURL, " - Key: ", Key, " - Secret: ", Secret));

                //adding new key & secret in Accounts DB
                DAL db = new DAL("Accounts");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@SubDomain", SubDomain),
                    new SqlParameter("@Key", Key),
                    new SqlParameter("@Secret", Secret)
                };

                db.execQuery("Set_APIKey", CommandType.StoredProcedure, parameters);

                //Creating New WebHook on subdomain.eralive.net
                string WebHookResponse = CreateWebHook(SiteURL, SubDomain, Key, Secret);

                string Response;

                //If web hook creation fails
                if (WebHookResponse.Contains("Error"))
                    Response = WebHookResponse;
                //If site does not exist
                else if (WebHookResponse.Contains("<!DOCTYPE html>"))
                    Response = "<h1>Error</h1><br/>Site does not exist, webhook creation failed!";
                //If db insert fails
                else if (db.ExceptionMsg != null)
                    Response = string.Concat("Error! ", db.ExceptionMsg);

                else
                    Response = "<h1>Success!</h1><br/>New API Key saved. New WebHook created!";

                return Response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string CreateWebHook(string WebSite, string slug, string Key, string Secret)
        {
            string apiPath = string.Concat(WebSite, "/wp-json/wc/v3/webhooks");
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters.Add("consumer_key", Key);
            Parameters.Add("consumer_secret", Secret);
            Parameters.Add("name", "OrderToEra");
            Parameters.Add("topic", "order.created");
            Parameters.Add("delivery_url", "https://" + slug + ".eraconnect.net/ERAAPI/api/Woo/OrderCreated");

            return App_Code.Helper.CallAPI(apiPath, Method.POST, Parameters);
        }

        public void SetCredentials(string slug, string Username)
        {
            DAL db = new DAL(slug);
            db.execQuery("update users set user_name='" + Username + "', Password='" + Username + "', isFirstLogin=1", CommandType.Text, null);
        }


        //Code graveyard
        //[HttpPost]
        //[ActionName("NewAPIKey")]
        //public void NewAPIKey([FromBody] App_Code.NewAPIKey_Data data)
        //{
        //    DAL db = new DAL("Accounts");
        //    SqlParameter[] para3 =
        //        {
        //            new SqlParameter("@log", JsonConvert.SerializeObject(data)),
        //            new SqlParameter("@controllerName", "SignUp"),
        //            new SqlParameter("@action", "NewAPIKey")
        //        };
        //    db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para3);
        //}

        //[HttpGet]
        //[ActionName("CreateNewAPIKey")]
        //public string CreateNewAPIKey(string WebSite)
        //{
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    parameters.Add("app_name", "EraConnect");
        //    parameters.Add("user_id", "995");
        //    parameters.Add("scope", "read_write");
        //    parameters.Add("return_url", "https://eralive.net");
        //    parameters.Add("callback_url", "https://eraconnect.net/signup/api/signup/NewAPIKey/");

        //    string apipath = string.Concat(WebSite, "/wc-auth/v1/authorize");
        //    string response = CallAPI(apipath, Method.POST, false, null, parameters);

        //    DAL db = new DAL("Accounts");
        //    SqlParameter[] para3 =
        //        {
        //            new SqlParameter("@log", response),
        //            new SqlParameter("@controllerName", "SignUp"),
        //            new SqlParameter("@action", "NewAPIKey")
        //        };
        //    db.execQuery("Set_ApiLog", CommandType.StoredProcedure, para3);
        //    return response;

        //}
    }
}
