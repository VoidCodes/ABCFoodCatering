using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABCFoodCatering.Controllers
{
    public class AdminController : Controller
    {
       public readonly RoleManager<IdentityRole> roleManager;
       public AdminController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult ViewRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
    }
}