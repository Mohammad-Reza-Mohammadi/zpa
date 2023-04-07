﻿using Entities.BaseEntityFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Orders
{
    public class OrderDetail : IntBaseEntity
    {

        public decimal StarCargo { get; set; }
        public int CountCargo { get; set; }


        public int OrderId { get; set; }
        public int CargoId { get; set; }

        public Order Order { get; set; }
        public Cargo.Cargo Cargo { get; set; }
    }
}
