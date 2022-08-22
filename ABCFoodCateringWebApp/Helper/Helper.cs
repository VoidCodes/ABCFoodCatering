using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ABCFoodCateringWebApp.Helper
{
    public class ABCFoodCateringAPI
    {
        public HttpClient Initial()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44352/");
            return client;
        }
    }
}
