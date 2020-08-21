using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace TuEnvioUtils
{
    public class Department
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }

        public Store Store { get; set; }

        public IEnumerable<Product> Products
        {
            get
            {
                if (_products == null)
                    UpdateProducts();
                return _products;
            }
        }

        private IEnumerable<Product> _products = null;

        public void UpdateProducts()
        {
            CultureInfo provider = new CultureInfo("en-GB");
            List<Product> answer = new List<Product>();
            IEnumerable<HtmlNode> prods;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(Url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("postman-token", "864b7b81-2d7b-278d-4985-65de84e54d51");
            request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);
            if ((prods = doc.DocumentNode.SelectNodes(".//div[@id='mainPanel']/div/div/div[@class='row']/ul/li")) != null)
            {
                foreach (HtmlNode node in prods)
                {
                    try
                    {
                        answer.Add(new Product()
                        {
                            Picture = node.SelectSingleNode("./div[@class='thumbnail']/a/object").Attributes["data"].DeEntitizeValue,
                            Name = node.SelectSingleNode("./div[@class='thumbSetting']/div[@class='thumbTitle']/a").InnerText,
                            Price = decimal.Parse(node.SelectSingleNode("./div[@class='thumbSetting']/div[@class='thumbPrice']/span").InnerText.Replace("$", "").Replace(" CUC", ""), provider),
                            URL = Store.url + "/" + node.SelectSingleNode("./div[@class='thumbSetting']/div[@class='thumbTitle']/a").Attributes["href"].DeEntitizeValue,
                            Department = this,
                        });
                    }
                    catch
                    {

                    }
                }
            };

            _products = answer;
        }
    }
}
