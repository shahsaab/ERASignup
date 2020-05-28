using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace ERASignup.Controllers
{
    public class SignUpController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5

        public string NewSite([FromBody] string Value)
        {
            DAL db = new DAL();
            string SubDomain = "check";
            var data = Request.RequestUri.ParseQueryString();
            if (data.HasKeys())
            {
                for (int i = 0; i < data.Keys.Count; i++)
                { Value += string.Concat(", ", data.AllKeys[i], ": ", data.Keys[i]); }
            }
            db.execQuery("insert into apiLogs(apilog,controllername,action) values('" + Value + "','SignUp','NewSite');", CommandType.Text, null);
            db.execQuery("insert into UserAccounts(SubDomain) values('" + SubDomain + "');", CommandType.Text, null);

            return "Yeah!";
        }

        public string Get(string id)
        {
            DAL db = new DAL();
            string SubDomain = "check";
            db.execQuery("insert into apiLogs(apilog,controllername,action) values('" + id + "','SignUp','Get');", CommandType.Text, null);
            db.execQuery("insert into UserAccounts(SubDomain) values('" + SubDomain + "');", CommandType.Text, null);

            return "Yeah!";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
            DAL db = new DAL();
            string SubDomain = "check";
            db.execQuery("insert into apiLogs(apilog,controllername,action) values('" + value + "','SignUp','Post');", CommandType.Text, null);
            db.execQuery("insert into UserAccounts(SubDomain) values('" + SubDomain + "');", CommandType.Text, null);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

    }
}