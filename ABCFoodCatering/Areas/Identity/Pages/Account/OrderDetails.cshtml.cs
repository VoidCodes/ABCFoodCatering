using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ABCFoodCatering.Models;

namespace ABCFoodCatering.Areas.Identity.Pages.Account
{
    public class OrderDeetailsModel : PageModel
    {
        private readonly ABCFoodCatering.Models.ApplicationDbContext _context;

        public OrderDeetailsModel(ABCFoodCatering.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.Order.FirstOrDefaultAsync(m => m.OrderID == id);

            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
