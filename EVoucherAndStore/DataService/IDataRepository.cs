using EVoucherAndStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStore.DataService
{
    public interface IDataRepository
    {
        Task<LoginResponse> Login(string userName, string password, string baseUrl);
        Task<List<VoucherModel>> GetVouchers(string token, string baseUrl);
        Task<List<VoucherModel>> GetVoucher(string token, int id, string baseUrl);
        Task<List<PaymentMethods>> GetPaymentMethods(string token, string baseUrl);
        Task<string> CreateVoucher(string token, VoucherModel model, string baseUrl);
        Task<List<TransactionModel>> GetTransactionsByPhoneNumber(string token, string phoneNumber, string baseUrl);
        Task<List<TransactionModel>> GetTransactionsById(string token, string id, string baseUrl);
        Task<string> BuyVoucher(string token, TransactionModel model, string baseUrl);
    }
}
