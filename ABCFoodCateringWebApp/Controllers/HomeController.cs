using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ABCFoodCateringWebApp.Models;
using ABCFoodCateringWebApp.Helper;
using System.Net.Http;
using Newtonsoft.Json;

namespace ABCFoodCateringWebApp.Controllers
{
    public class HomeController : Controller
    {
        // Declare api to use
        ABCFoodCateringAPI _api = new ABCFoodCateringAPI();
        public async Task<IActionResult> Index()
        {
            List<OrderData> orders = new List<OrderData>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/orders");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                // Deserialize json data
                orders = JsonConvert.DeserializeObject<List<OrderData>>(results);
            }
            return View(orders);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
