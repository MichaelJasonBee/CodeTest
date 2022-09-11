using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PCMS
{
    class Program
    {
        private static string username;
        private static string password;
        private static string baseUrl;
        private static string token;
        static IConfigurationRoot configuration;
        static void Main(string[] args)
        {
            Log("BEGIN");
            configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false)
                .Build();

            username = configuration["UserName"];
            password = configuration["Password"];
            baseUrl = configuration["BaseUrl"];

            Run().Wait();
            Log("END");
        }

        static async Task Run()
        {
            try
            {
                var tokenResponse = await GetToken(username, password);
                token = tokenResponse;

                if (string.IsNullOrEmpty(token))
                    return;

                var paymentMethods = await GetPaymentMethods(token);

                if (!paymentMethods.Any())
                    return;

                await GenerateVoucherAndBuy(token, paymentMethods);
            }
            catch (Exception ex)
            {
                Log("ERROR at Main");
                Log(ex.ToString());
            }
        }

        private static async Task<string> GetToken(string userName, string password)
        {
            try
            {
                using (var _httpClient = new HttpClient())
                {
                    string json = "{\"username\":\"" + userName + "\"," +
                                  "\"password\":\"" + password + "\"}";
                    var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{baseUrl}/api/Login", requestContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(content);
                        var token = result.AccessToken;
                        return token;
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Log("ERROR at getting token");
                        Log(content);
                        return "";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Log("ERROR at getting token");
                Log(ex.ToString());
                return "";
            }
            catch (Exception ex)
            {
                Log("ERROR at getting token");
                Log(ex.ToString());
                return "";
            }
        }

        private static async Task<List<PaymenMethodModel>> GetPaymentMethods(string token)
        {
            try
            {
                var result = new List<PaymenMethodModel>();
                string tokens = token;
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{tokens}");
                    var response = await _httpClient.GetAsync($"{baseUrl}/api/evoucher/GetPaymentMethods");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        result = JsonConvert.DeserializeObject<List<PaymenMethodModel>>(content);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log("No payment methods are found");
                return new List<PaymenMethodModel>();
            }
        }

        private static async Task GenerateVoucherAndBuy(string token, List<PaymenMethodModel> paymenMethods)
        {
            try
            {
                var result = string.Empty;
                for (var i = 0; i < Convert.ToInt32(configuration["NumberOfVoucherToCreate"]); i++)
                {
                    var voucherTitle = "PSM_" + "_" + (i + 1) + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmssffff");
                    var voucherId = await CreateVoucher(token, paymenMethods, voucherTitle);

                    var buyers = configuration.GetSection("Buyers").Get<BuyersModel>();
                    if (buyers?.Buyers == null)
                        return;

                    if (!buyers.Buyers.Any())
                        return;

                    foreach (var buyer in buyers.Buyers)
                    {
                        buyer.VoucherId = voucherId;
                        buyer.BuyType = buyer.IsGift ? 1 : 0;
                        await BuyVouchers(token, buyer);
                    }
                }

            }
            catch (Exception ex)
            {
                Log("ERROR at GenerateVoucherAndBuy");
                Log(ex.ToString());
            }
        }

        private static async Task<int> CreateVoucher(string token, List<PaymenMethodModel> paymenMethods, string voucherTitle)
        {
            var accessToken = token;
            var voucherId = 0;
            var voucherModel = new VoucherModel()
            {
                Amount = 1000,
                AvailablePaymentMethods = string.Join(",", paymenMethods.Select(x => x.PaymentMethodId)),
                Title = voucherTitle,
                Description = "Created from PSM",
                DiscountPaymentMethodId = paymenMethods.FirstOrDefault().PaymentMethodId,
                ExpiryDate = DateTime.Now.AddMonths(Convert.ToInt32(configuration["Voucher:ExpiryDay"])),
                GiftPerUserLimit = Convert.ToInt32(configuration["Voucher:GiftPerUserLimit"]),
                IsActive = Convert.ToBoolean(configuration["Voucher:IsActive"]),
                MaxVoucherLimit = Convert.ToInt32(configuration["Voucher:GiftPerUserLimit"]),
                Quantity = Convert.ToInt32(configuration["Voucher:Quantity"]),
            };

            try
            {
                using (var _httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(voucherModel);
                    var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{accessToken}");
                    var response = await _httpClient.PostAsync($"{baseUrl}/api/evoucher/UpdateVoucher", requestContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        voucherId = Convert.ToInt32(content);
                        Log($"Created Voucher Id : {voucherId}");
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Log("ERROR At Creating Voucher");
                        Log(content);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Log("ERROR At Creating Voucher");
                Log(ex.ToString());
            }
            catch (Exception ex)
            {
                Log("ERROR At Creating Voucher");
                Log(ex.ToString());
            }

            return voucherId;
        }

        private static async Task BuyVouchers(string token, BuyerModel buyer)
        {
            var accessToken = token;
            try
            {
                using (var _httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(buyer);
                    var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{accessToken}");
                    var response = await _httpClient.PostAsync($"{baseUrl}/api/evoucher/BuyVoucher", requestContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseJson = JsonConvert.DeserializeObject<BuyResponseModel>(content);
                        Log($"Phone Number : {buyer.PhoneNumber}, Number of Bought Vouchers : {responseJson.NumberOfVouchersCreated}");
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Log($"ERROR at buying vouchers for Phone Number : {buyer.PhoneNumber}");
                        Log(content);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Log($"ERROR at buying vouchers for Phone Number : {buyer.PhoneNumber}");
                Log(ex.ToString());
            }
            catch (Exception ex)
            {
                Log($"ERROR at buying vouchers for Phone Number : {buyer.PhoneNumber}");
                Log(ex.ToString());
            }
        }

        static void Log(string message)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Log");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string dateTime = DateTime.Now.ToString("yyyy_MM_dd");
            string fullPath = Path.Combine(path, $"log_{dateTime}.txt");
            Console.WriteLine(message);
            using (StreamWriter w = File.AppendText(fullPath))
            {
                w.Write("\r\nLog Entry : ");
                w.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.Write("  :");
                w.Write($"  :{message}");
            }
        }
    }
}
