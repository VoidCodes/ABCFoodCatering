using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ABCFoodCatering.Models;

namespace ABCFoodCatering.Areas.Identity.Pages.Account.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ABCFoodCatering.Models.ApplicationDbContext _context;

        public IndexModel(ABCFoodCatering.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; }

        public async Task OnGetAsync()
        {
            Order = await _context.Order.ToListAsync();
        }
    }
}
