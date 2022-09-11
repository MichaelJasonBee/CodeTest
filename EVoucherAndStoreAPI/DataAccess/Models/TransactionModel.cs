using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.DataAccess.Models
{
    [Index(nameof(PromoCode), IsUnique = true)]
    public class TransactionModel
    {
        [Key]
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public BuyType BuyType { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PromoCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        [ForeignKey(nameof(VoucherId))]
        public EVouchers Vouchers { get; set; }
    }

    public enum BuyType
    {
        ForMySelf,
        Gift
    }
}
