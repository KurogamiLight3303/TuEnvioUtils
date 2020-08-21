using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TuEnvioUtils
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; }

        public string URL { get; set; }

        public Department Department { get; set; }

        public bool IsAvaliable
        {
            get
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var client = new RestClient(URL);
                var request = new RestRequest(Method.GET);
                request.AddHeader("postman-token", "864b7b81-2d7b-278d-4985-65de84e54d51");
                request.AddHeader("cache-control", "no-cache");
                IRestResponse response = client.Execute(request);
                if (response.Content.Contains("product-inputs"))
                    return true;
                return false;
            }
        }
    }
}
