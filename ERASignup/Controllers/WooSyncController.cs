using ERASignup.App_Code;
using System;
using System.Web.Http;

namespace ERASignup.Controllers
{
    public class WooSyncController : ApiController
    {
        [HttpGet]
        [ActionName("SyncSite")]
        public bool SyncSite(string SubDomain, string APIKey, string APISecret, string APIURL, DateTime LastSyncedAt, bool CleanBeforeSync)
        {
            WooSyncLogic logic = new WooSyncLogic();

            bool success = true;
            
            if (CleanBeforeSync)
                success = logic.DeleteAllProducts(APIKey, APISecret, APIURL);

            if (success)
                success = logic.SyncProducts(SubDomain, APIKey, APISecret, APIURL);

            if (success)
                success = logic.SyncOrders(SubDomain, APIKey, APISecret, APIURL, LastSyncedAt);

            if (success)
                success = logic.SyncDelivery_Shipping(SubDomain, APIKey, APISecret, APIURL, LastSyncedAt);

            if (success)
                logic.SetSyncLog(SubDomain, "Sync Process Completed!", success);

            else
                logic.SetSyncLog(SubDomain, logic.error, success);

            return success;
        }
    }
}
