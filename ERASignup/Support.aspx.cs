using ERASignup.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERASignup
{
    public partial class Support : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                GetAccounts();
        }

        public void GetAccounts()
        {
            DAL db = new DAL("Accounts");
            System.Data.DataTable dt = db.execQuery("select subdomain, SiteURL from userAccounts where Status=1", System.Data.CommandType.Text, null);

            if (dt == null || dt.Rows.Count == 0)
            {
                btnAdd.Enabled = false;
                return;
            }

            ddSubDomains.DataSource = dt;
            ddSubDomains.DataTextField = "SubDomain";
            ddSubDomains.DataValueField = "SiteURL";
            ddSubDomains.DataBind();
            ddSubDomains.Items.Insert(0, new ListItem("Select", "0"));
            btnAdd.Enabled = true;
        }

        public void Refresh()
        {
            GetAccounts();
            txtKey.Text = string.Empty;
            txtSecret.Text = string.Empty;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (ddSubDomains.SelectedIndex == 0)
            {
                responseRow.Visible = true;
                txtResponse.Text = "<h1>Wait...</h1><br/>Select a site first!";
                txtResponse.ForeColor =  System.Drawing.Color.Yellow;
                return;
            }
            string SubDomain = ddSubDomains.SelectedItem.Text;
            string SiteURL = ddSubDomains.SelectedItem.Value;

            SignUpController sign = new SignUpController();

            string Response = sign.SetAPIKey(SiteURL, SubDomain, txtKey.Text, txtSecret.Text);

            responseRow.Visible = true;
            txtResponse.Text = Response;
            txtResponse.ForeColor = Response.Contains("Error") ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
        }
    }
}