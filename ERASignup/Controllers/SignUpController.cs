using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            if (db.ExceptionMsg == null)
                return "Yeah!";
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
    }
}
