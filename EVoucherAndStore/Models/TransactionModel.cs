using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStore.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        [Required]
        [Display(Name = "Buy Type")]
        public BuyType BuyType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Promo Code")]
        public string PromoCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsUsed { get; set; }
        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Display(Name = "Payment Methods")]
        public string PaymentMethods { get; set; }
        public VoucherModel Vouchers { get; set; }
    }
    public enum BuyType
    {
        ForMySelf,
        Gift
    }

    public class BuyVoucherResponse
    {
        public List<string> Vouchers { get; set; }
    }
}
