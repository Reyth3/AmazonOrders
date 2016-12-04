using AmazonOrders.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmazonOrders
{
    class OrdersManager
    {
        public static async Task<List<Order>> GetOrders(string url)
        {
            using (var http = new HttpClient())
            using (var res = await http.GetAsync(url))
                if(res.IsSuccessStatusCode)
                {
                    var html = await res.Content.ReadAsStringAsync();

                    // Use HtmlAgilityPack library to quickly parse the requested HTML
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    // Each order's info is stored in a div with class atribute equal to 'a-box-group a-spacing-base order', so we can use LINQ to query that
                    var orders = doc.DocumentNode.Descendants("div").Where(o => o.GetAttributeValue("class", "") == "a-box-group a-spacing-base order").ToList();
                    // Now, parse each order and assign the values to objects, which will make it easier to access later
                    var list = new List<Order>();
                    foreach(var ord in orders)
                    {
                        var placed = ord.Descendants("div").Where(o => o.GetAttributeValue("class", "") == "a-column a-span3").First().Descendants("span").Last().InnerText.Replace("\r\n", "").Trim();
                        var total = ord.Descendants("div").Where(o => o.GetAttributeValue("class", "") == "a-column a-span2").First().Descendants("span").Last().InnerText.Replace("\r\n", "").Trim();
                        var itemsList = new List<Item>();
                        // With these class names it is needlessly confusing to figure out what's what. Unfortunately there is no easier and reliable way to do this. I can only imagine how much work that'd take with regex.
                        var items = ord.Descendants("div").Where(o => o.GetAttributeValue("class", "").Contains("a-box shipment")).First().Descendants("div").Where(o => Regex.IsMatch(o.GetAttributeValue("class", ""), "a-fixed-left-grid a-spacing.*")).ToList();
                        foreach(var it in items)
                        {
                            // Helper variable for quick access to rows
                            var rows = it.Descendants("div").Where(o => o.GetAttributeValue("class", "") == "a-row").ToList();
                            // Item information
                            var itName = rows[0].InnerText.Replace("\r\n", "").Trim();
                            var itSoldBy = rows[1].InnerText.Split(':')[1].Replace("\r\n", "").Trim();
                            var itPrice = rows[3].InnerText.Replace("\r\n", "").Trim();

                            // Now titme to add all that to an Item object
                            itemsList.Add(new Item()
                            {
                                Name = WebUtility.HtmlDecode(itName), // This function is necessary to decode html-specific encoding (like &amp; = &)
                                SoldBy = itSoldBy,
                                Price = itPrice,
                            });
                        }
                        list.Add(new Order() { Items = itemsList, Placed = placed, Total = total });
                    }
                    return list;
                }
            return null;
        }
    }
}
