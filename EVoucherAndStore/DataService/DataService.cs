using EVoucherAndStore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EVoucherAndStore.DataService
{
    public class DataRepository : IDataRepository
    {
        public async Task<List<VoucherModel>> GetVouchers(string token, string baseUrl)
        {
            var result = new List<VoucherModel>();
            string tokens = token;
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                var response = await _httpClient.GetAsync($"{baseUrl}/api/evoucher/GetVouchers");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    result = JsonConvert.DeserializeObject<List<VoucherModel>>(content);
                }
                
            }
            return result;
        }

        public async Task<List<VoucherModel>> GetVoucher(string token, int id, string baseUrl)
        {
            var result = new List<VoucherModel>();
            string tokens = token;
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                var response = await _httpClient.GetAsync($"{baseUrl}/api/evoucher/GetVoucher/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    result = JsonConvert.DeserializeObject<List<VoucherModel>>(content);
                }

            }
            return result;
        }

        public async Task<LoginResponse> Login(string userName, string password, string url)
        {
            var result = new LoginResponse();
            string baseUrl = url;
            string token = "";
            using (var _httpClient = new HttpClient())
            {
                string json = "{\"username\":\"" + userName + "\"," +
                              "\"password\":\"" + password + "\"}";
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{baseUrl}/api/Login", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<LoginResponse>(content);
                }
            }
            return result;
        }

        public async Task<List<PaymentMethods>> GetPaymentMethods(string token, string baseUrl)
        {
            var result = new List<PaymentMethods>();
            string tokens = token;
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                var response = await _httpClient.GetAsync($"{baseUrl}/api/evoucher/GetPaymentMethods");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    result = JsonConvert.DeserializeObject<List<PaymentMethods>>(content);
                }

            }
            return result;
        }

        public async Task<string> CreateVoucher(string token, VoucherModel model, string baseUrl)
        {
            var result = string.Empty;
            string tokens = token;
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                string json = JsonConvert.SerializeObject(model);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{baseUrl}/api/evoucher/UpdateVoucher", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return "Successful";// result = JsonConvert.DeserializeObject<List<PaymentMethods>>(content);
                }
                else
                {
                    return "Failed";
                }
            }
            return result;
        }

        public async Task<List<TransactionModel>> GetTransactionsByPhoneNumber(string token, string phoneNumber, string baseUrl)
        {
            var result = new List<TransactionModel>();
            string tokens = token;
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                var response = await _httpClient.GetAsync($"{baseUrl}/api/evoucher/GetTransactions/{phoneNumber}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    result = JsonConvert.DeserializeObject<List<TransactionModel>>(content);
                }

            }
            return result;
        }

        public async Task<List<TransactionModel>> GetTransactionsById(string token, string id, string baseUrl)
        {
            var result = new List<TransactionModel>();
            string tokens = token;
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                var response = await _httpClient.GetAsync($"{baseUrl}/api/evoucher/GetTransaction/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    result = JsonConvert.DeserializeObject<List<TransactionModel>>(content);
                }

            }
            return result;
        }

        public async Task<string> BuyVoucher(string token, TransactionModel model, string baseUrl)
        {
            var result = string.Empty;
            string tokens = token;
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                string json = JsonConvert.SerializeObject(model);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{baseUrl}/api/evoucher/BuyVoucher", requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var resultObj = JsonConvert.DeserializeObject<BuyVoucherResponse>(content);
                    return "Created Promocodes are : " + string.Join(',', resultObj.Vouchers);
                }
                else
                {
                    return "Failed";
                }
            }
            return result;
        }

    }
}
