using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public string HmProductId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int? Quantity { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }
    }
}
