using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ABCFoodCatering.Models
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}