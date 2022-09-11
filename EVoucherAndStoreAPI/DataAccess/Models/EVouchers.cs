using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.DataAccess.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public class EVouchers
    {
        [Key]
        public int Id { get; set; }
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
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        [ForeignKey(nameof(DiscountPaymentMethodId))]
        public PaymenMethods Paymentmethods { get; set; }
    }
}
