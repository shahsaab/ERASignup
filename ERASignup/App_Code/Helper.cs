using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERASignup.App_Code
{
    public class Helper
    {
        public static string CallAPI(string apiPath, Method method, Dictionary<string, string> Parameters = null)
        {
            var client = new RestClient(apiPath);

            var request = new RestRequest(method);
            if (Parameters != null)
                foreach (KeyValuePair<string, string> para in Parameters)
                    request.AddQueryParameter(para.Key, para.Value);

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                return response.Content;
            else
                return "Error: " + response.Content;
        }


    }
}