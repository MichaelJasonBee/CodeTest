using EVoucherAndStore.DataService;
using EVoucherAndStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStore.Controllers
{
    public class EVouchersController : Controller
    {
        private IHttpContextAccessor Accessor;
        private readonly ILogger<EVouchersController> _logger;
        private readonly IDataRepository _service;
        private readonly IConfiguration _config;
        private static string token;
        public EVouchersController(ILogger<EVouchersController> logger, IDataRepository service, IConfiguration config, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _service = service;
            _config = config;
            Accessor = accessor;
            token = this.Accessor.HttpContext.Request.Cookies["token"];
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetVouchers(token, _config.GetValue<string>("BaseUrl"));

            return View(model?.OrderByDescending(x => x.Id));
        }

        public async Task<ViewResult> Create()
        {
            var paymentMethods = await _service.GetPaymentMethods(token, _config.GetValue<string>("BaseUrl"));

            ViewBag.Payments = new SelectList(paymentMethods, "PaymentMethodId", "PaymentMethodName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VoucherModel model)
        {
            var result = await _service.CreateVoucher(token, model, _config.GetValue<string>("BaseUrl"));
            var paymentMethods = await _service.GetPaymentMethods(token, _config.GetValue<string>("BaseUrl"));
            ViewBag.Payments = new SelectList(paymentMethods, "PaymentMethodId", "PaymentMethodName");

            TempData["SubmitMessage"] = result;
            return View();
        }

        public async Task<ViewResult> Details(int id)
        {
            var result = await _service.GetVoucher(token, id, _config.GetValue<string>("BaseUrl"));
            var model = result?.FirstOrDefault();
            return View(model);
        }

        public async Task<ViewResult> Edit(int id)
        {
            var paymentMethods = await _service.GetPaymentMethods(token, _config.GetValue<string>("BaseUrl"));
            
            var result = await _service.GetVoucher(token, id, _config.GetValue<string>("BaseUrl"));

            ViewBag.Payments = new SelectList(paymentMethods, "PaymentMethodId", "PaymentMethodName");

            var model = result?.FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VoucherModel model)
        {
            var result = await _service.CreateVoucher(token, model, _config.GetValue<string>("BaseUrl"));
            var paymentMethods = await _service.GetPaymentMethods(token, _config.GetValue<string>("BaseUrl"));
            ViewBag.Payments = new SelectList(paymentMethods, "PaymentMethodId", "PaymentMethodName");

            TempData["SubmitMessage"] = result;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
