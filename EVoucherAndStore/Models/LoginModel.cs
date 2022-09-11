using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStore.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
