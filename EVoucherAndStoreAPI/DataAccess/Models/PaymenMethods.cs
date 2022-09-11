using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.DataAccess.Models
{
    [Index(nameof(PaymentMethodName), IsUnique = true)]
    public class PaymenMethods
    {
        [Key]
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string Description { get; set; }
        //public ICollection<EVouchers> EVouchers { get; set; }
    }
}
