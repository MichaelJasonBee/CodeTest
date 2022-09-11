using EVoucherAndStoreAPI.Cache;
using EVoucherAndStoreAPI.DataAccess;
using EVoucherAndStoreAPI.DataAccess.Models;
using EVoucherAndStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EVoucherController : ControllerBase
    {
        private readonly MySqlContext _context;
        private readonly ICacheService _cacheService;
        public EVoucherController(MySqlContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        #region Vouchers

        [HttpGet("GetVouchers")]
        [Authorize(Roles = "Administrator, Buyer")]
        [Cached(60)]
        public IActionResult GetVouchers()
        {
            try
            {                
                return Ok(_context.EVouchers.Include(x => x.Paymentmethods).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpGet("GetVoucher/{number}")]
        [Authorize(Roles = "Administrator, Buyer")]
        [Cached(60)]
        public IActionResult GetVoucherById(int number)
        {
            try
            {
                if (number == 0)
                    return BadRequest("Invalid request");

                return Ok(_context.EVouchers.Where(x => x.Id == number).Include(x => x.Paymentmethods).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpPost("UpdateVoucher")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SaveVoucher([FromBody] EVouchers request)
        {
            try
            {
                if (request is null)
                    return BadRequest();

                var id = 0;
                if (request.Id == 0)
                {
                    request.CreatedOn = DateTime.Now;
                    request.UpdatedOn = DateTime.Now;
                    _context.EVouchers.AddAsync(request);
                    _context.SaveChanges();
                    id = request.Id;
                }
                else
                {
                    if (!_context.EVouchers.ToList().Any())
                        return BadRequest("No voucher found");

                    var voucher = _context.EVouchers.FirstOrDefault(x => x.Id == request.Id);
                    if (voucher is null)
                        return BadRequest("No voucher found");

                    voucher.UpdatedOn = DateTime.Now;
                    voucher.Title = request.Title;
                    voucher.Description = request.Description;
                    voucher.ExpiryDate = request.ExpiryDate;
                    voucher.Image = request.Image;
                    voucher.Amount = request.Amount;
                    voucher.DiscountAmount = request.DiscountAmount;
                    voucher.AvailablePaymentMethods = request.AvailablePaymentMethods;
                    voucher.DiscountPaymentMethodId = request.DiscountPaymentMethodId;
                    voucher.Quantity = request.Quantity;
                    voucher.MaxVoucherLimit = request.MaxVoucherLimit;
                    voucher.GiftPerUserLimit = request.GiftPerUserLimit;
                    voucher.IsActive = request.IsActive;
                    _context.SaveChanges();
                    id = voucher.Id;
                }

                await _cacheService.RemoveCache("/api/evoucher/GetVouchers");
                await _cacheService.RemoveCache($"/api/evoucher/GetVoucher/{id}");
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        #endregion

        #region Payment Methods

        [HttpGet("GetPaymentMethods")]
        [Authorize(Roles = "Administrator, Buyer")]
        [Cached(60)]
        public IActionResult GetPaymentMethods()
        {
            try
            {
                return Ok(_context.PaymentMethods.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpPost("UpdatePaymentMethod")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SavePaymentMethod([FromBody] PaymenMethods request)
        {
            try
            {
                if (request is null)
                    return BadRequest();

                if (string.IsNullOrEmpty(request.PaymentMethodName))
                    return BadRequest("Name is required");

                if (string.IsNullOrEmpty(request.Description))
                    return BadRequest("Description is required");

                if (request.PaymentMethodId == 0)
                {
                    _context.PaymentMethods.AddAsync(request);
                    _context.SaveChanges();
                }
                else
                {
                    if (!_context.PaymentMethods.ToList().Any())
                        return BadRequest("No Payment method found");

                    var payment = _context.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == request.PaymentMethodId);
                    if (payment is null)
                        return BadRequest("No Payment method found");

                    payment.Description = request.Description;
                    payment.PaymentMethodName = request.PaymentMethodName;
                    _context.SaveChanges();

                }

                await _cacheService.RemoveCache("/api/evoucher/GetPaymentMethods");
                return Ok("Updated Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex?.InnerException?.Message?.ToString());
            }
        }

        #endregion

        #region Store

        [HttpGet("GetAllTransactions")]
        [Authorize(Roles = "Administrator, Buyer")]
        [Cached(60)]
        public IActionResult GetAllTransactions()
        {
            try
            {
                return Ok(_context.Transactions.Include(x => x.Vouchers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpGet("GetTransactions/{number}")]
        [Authorize(Roles = "Administrator, Buyer")]
        [Cached(60)]
        public IActionResult GetTransactionsByPhoneNumber(string number)
        {
            try
            {
                if (string.IsNullOrEmpty(number))
                    return BadRequest("Invalid request");

                return Ok(_context.Transactions.Where(x => x.PhoneNumber == number).Include(x => x.Vouchers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpGet("GetTransaction/{number}")]
        [Authorize(Roles = "Administrator, Buyer")]
        [Cached(60)]
        public IActionResult GetTransactionById(int number)
        {
            try
            {
                if (number == 0)
                    return BadRequest("Invalid request");

                return Ok(_context.Transactions.Where(x => x.Id == number).Include(x => x.Vouchers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        [HttpPost("BuyVoucher")]
        [Authorize(Roles = "Administrator, Buyer")]
        public async Task<IActionResult> BuyVoucher([FromBody] BuyRequestModel request)
        {
            try
            {
                if (request is null)
                    return BadRequest();

                if (request.VoucherId == 0)
                    return BadRequest("Please select a voucher");

                if (request.Quantity == 0)
                    return BadRequest("Quantity is needed");

                var voucher = _context.EVouchers?.FirstOrDefault(x => x.Id == request.VoucherId);
                if (voucher is null)
                    return BadRequest("Invalid Voucher");

                var validToCreate = false;
                var transactions = _context.Transactions;
                if (transactions is null)
                {
                    if (request.BuyType == BuyType.ForMySelf && (voucher.MaxVoucherLimit >= request.Quantity))
                        validToCreate = true;

                    if (request.BuyType == BuyType.Gift && (voucher.GiftPerUserLimit >= request.Quantity))
                        validToCreate = true;
                }
                else
                {
                    var existingTransactions = transactions.Where(x => x.VoucherId == request.VoucherId);
                    var numberOfVouchers = existingTransactions.Count();
                    if (voucher.Quantity > numberOfVouchers)
                    {
                        var currentUserVouchers = existingTransactions.Where(x => x.PhoneNumber == request.PhoneNumber);
                        if (!currentUserVouchers.Any())
                        {
                            if (request.BuyType == BuyType.ForMySelf && (voucher.MaxVoucherLimit >= request.Quantity))
                                validToCreate = true;

                            if (request.BuyType == BuyType.Gift && (voucher.GiftPerUserLimit >= request.Quantity))
                                validToCreate = true;
                        }
                        else if (request.BuyType == BuyType.ForMySelf && (voucher.MaxVoucherLimit >= (currentUserVouchers.Count() + request.Quantity)))
                        {
                            validToCreate = true;
                        }
                        else if (request.BuyType == BuyType.Gift && (voucher.GiftPerUserLimit >= (currentUserVouchers.Count() + request.Quantity)))
                        {
                            validToCreate = true;
                        }
                    }
                }

                if (!validToCreate)
                    return StatusCode(500, "Limitation of buying this voucher is reached. Please try another voucher.");

                var result = new List<string>();
                for (var i = 0; i < request.Quantity; i++)
                {
                    var model = new TransactionModel();
                    model.BuyType = request.BuyType;
                    model.IsActive = true;
                    model.IsUsed = true;
                    model.Name = request.Name;
                    model.PhoneNumber = request.PhoneNumber;
                    model.VoucherId = request.VoucherId;
                    model.PromoCode = GeneratePromoCode();
                    model.CreatedOn = DateTime.Now;
                    model.UpdatedOn = DateTime.Now;
                    _context.Transactions.AddAsync(model);
                    _context.SaveChanges();
                    result.Add(model.PromoCode);

                    await _cacheService.RemoveCache($"/api/evoucher/GetTransactions/{model.PhoneNumber}");
                }
                await _cacheService.RemoveCache("/api/evoucher/GetAllTransactions");
                return Ok(new { Message = "Successful", NumberOfVouchersCreated = result.Count(), Vouchers = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex?.InnerException?.Message?.ToString());
            }
        }

        private string GeneratePromoCode()
        {
            return $"{GenerateSixDigit()}{GenerateFiveAlphabets()}";
        }

        private string GenerateSixDigit()
        {
            var chars = "0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        private string GenerateFiveAlphabets()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        #endregion
    }
}
