using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStore.Models
{
    public class PaymentMethods
    {
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string Description { get; set; }
    }
}
