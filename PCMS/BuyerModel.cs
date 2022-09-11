using System;
using System.Collections.Generic;
using System.Text;

namespace PCMS
{
    class BuyerModel
    {
        public int VoucherId { get; set; }
        public int BuyType { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int Quantity { get; set; }
        public bool IsGift { get; set; }
    }

    class BuyersModel
    {
        public List<BuyerModel> Buyers { get; set; }
    }
}
