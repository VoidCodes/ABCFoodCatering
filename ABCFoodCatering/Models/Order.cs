using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABCFoodCatering.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        [Required]
        public string FoodDescription { get; set; }
        public string DeliveryAddress { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DeliveryDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan DeliveryTime { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string ClientName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OrderDate { get; set; }
    }
}
