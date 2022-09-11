using EVoucherAndStore.DataService;
using EVoucherAndStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataRepository _service;
        private readonly IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IDataRepository service, IConfiguration config)
        {
            _logger = logger;
            _service = service;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind] LoginModel model)
        {
            var res = await _service.Login(model.UserName, model.Password, _config.GetValue<string>("BaseUrl"));

            if (string.IsNullOrEmpty(res.AccessToken))
            {
                TempData["Msg"] = "Invalid username and password!";
                return View();
            }
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Append("token", res.AccessToken, option);
            Response.Cookies.Append("phonenumber", res.PhoneNumber, option);
            if (!string.IsNullOrEmpty(res.Role) && res.Role.ToLower() == "administrator")
            {
                return RedirectToAction("Index", "EVouchers");
            }
            else
            {
                return RedirectToAction("Index", "Store");
            }
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
