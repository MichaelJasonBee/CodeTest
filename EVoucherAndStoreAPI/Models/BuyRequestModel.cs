using EVoucherAndStoreAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.Models
{
    public class BuyRequestModel
    {
        public int Quantity { get; set; }
        public int VoucherId { get; set; }
        public BuyType BuyType { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

    }
}
