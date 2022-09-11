using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStore.Models
{
    public class VoucherModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }
        public string Image { get; set; }
        [Required]
        public double Amount { get; set; }
        public string AvailablePaymentMethods { get; set; }
        [Required]
        [Display(Name = "Discount Payment Method")]
        public int DiscountPaymentMethodId { get; set; }
        [Required]
        [Display(Name = "Discount %")]
        public double DiscountAmount { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Display(Name = "Max Voucher Limit")]
        public int MaxVoucherLimit { get; set; }
        [Required]
        [Display(Name = "Gift Per User Limit")]
        public int GiftPerUserLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public PaymentMethods Paymentmethods { get; set; }
    }
}
