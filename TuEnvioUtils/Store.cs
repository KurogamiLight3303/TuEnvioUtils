using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TuEnvioUtils
{
    public class Store
    {
        public int id { get; set; }
        public string province { get; set; }
        public bool online { get; set; }
        public string name { get; set; }

        public bool pickUpOnStore { get; set; }

        public bool homeDelivery { get; set; }
        public bool freezeDelivery { get; set; }
        public string deliveryTime { get; set; }
        public string cost { get; set; }

        public string address { get; set; }

        public string email { get; set; }

        public string phone { get; set; }
        public string url { get; set; }

        public string cadena { get; set; }

        [JsonIgnore]
        public IEnumerable<Department> Departments
        {
            get
            {
                if (_deps == null)
                {
                    _deps = GetDepartments();
                }
                return _deps;
            }
        }

        private IEnumerable<Department> _deps = null;

        public static IEnumerable<Store> GetStores()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient("https://www.tuenvio.cu/stores.json");
            var request = new RestRequest(Method.GET);
            request.AddHeader("postman-token", "864b7b81-2d7b-278d-4985-65de84e54d51");
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);


            return JsonConvert.DeserializeObject<IEnumerable<Store>>(response.Content);
        }

        private IEnumerable<Department> GetDepartments()
        {
            List<Department> answer = new List<Department>();
            IEnumerable<HtmlNode> deps, roots;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(url + "/Products?depPid=0");
            var request = new RestRequest(Method.GET);
            request.AddHeader("postman-token", "864b7b81-2d7b-278d-4985-65de84e54d51");
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response.Content);
                if ((roots = doc.DocumentNode.SelectNodes(".//div[@class='mainNav']/div/div/ul/li")) != null)
                    foreach (HtmlNode node in roots)
                    {
                        if ((deps = node.SelectNodes("./div/ul/li/a")) != null)
                            foreach (HtmlNode dep in deps)
                            {
                                answer.Add(new Department()
                                {
                                    Name = dep.InnerText,
                                    Url = url + "/" + dep.Attributes["href"].DeEntitizeValue,
                                    Category = node.InnerText,
                                    Store = this,
                                });
                            }
                    }
            }
            return answer;
        }
    }
}
