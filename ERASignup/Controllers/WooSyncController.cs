﻿using ERASignup.App_Code;
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

            logic.SetSyncLog(SubDomain, "Sync Process Started by eSync Service!", success);

            if (CleanBeforeSync)
                success = logic.DeleteAllProducts(SubDomain, APIKey, APISecret, APIURL);

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

        [HttpGet]
        [ActionName("SyncThisSite")]
        public string SyncThisSite(string SubDomain)
        {
            string APIKey, APISecret, APIURL;
            DateTime LastSyncedAt = new DateTime(); bool CleanBeforeSync;

            DAL db = new DAL("Accounts");
            System.Data.DataTable subdomains = db.execQuery("Get_AccountsForSync '" + SubDomain + "'", System.Data.CommandType.Text, null);

            if (subdomains == null || subdomains.Rows.Count == 0)
                return "Error: Unable to sync... please setup ERA License Key. For help, contact support@eralive.net";

            SubDomain = subdomains.Rows[0]["SubDomain"].ToString();
            APIKey = subdomains.Rows[0]["API_Key"].ToString();
            APISecret = subdomains.Rows[0]["API_Secret"].ToString();
            APIURL = subdomains.Rows[0]["APIURL"].ToString();
            DateTime.TryParse(subdomains.Rows[0]["LastSyncedAt"].ToString(), out LastSyncedAt);
            CleanBeforeSync = subdomains.Rows[0]["CleanBeforeSync"].ToString() == "1";

            WooSyncLogic logic = new WooSyncLogic();

            bool success = true;

            logic.SetSyncLog(SubDomain, "Sync Process Started by User (On-Demand)!", success);

            if (CleanBeforeSync)
                success = logic.DeleteAllProducts(SubDomain, APIKey, APISecret, APIURL);

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

            return success ? "Data has been synced successfully!" : "Error: Unable to sync. Please try again... For help, contact support@eralive.net";
        }
    }
}
