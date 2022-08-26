using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace ABCFoodCatering.Areas.Identity.Pages.Account
{
    public class ViewRolesModel : PageModel


    {
        // Add Identity User
        public IEnumerable<IdentityRole> IdentityRoles { get; set; }
       
        public void OnGet()
        {
           //
        }
    }
}