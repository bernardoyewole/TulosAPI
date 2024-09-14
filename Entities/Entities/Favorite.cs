﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string HmProductId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }
    }
}
