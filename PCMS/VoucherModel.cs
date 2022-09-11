using System;
using System.Collections.Generic;
using System.Text;

namespace PCMS
{
    class VoucherModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Image { get; set; }
        public double Amount { get; set; }
        public string AvailablePaymentMethods { get; set; }
        public int DiscountPaymentMethodId { get; set; }
        public double DiscountAmount { get; set; }
        public int Quantity { get; set; }
        public int MaxVoucherLimit { get; set; }
        public int GiftPerUserLimit { get; set; }
        public bool IsActive { get; set; }
    }
}
