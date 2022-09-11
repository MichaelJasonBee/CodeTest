using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.Models
{
    public class UserConstant
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel() { MobilePhone = "09975722061", Username = "admin", EmailAddress = "jason.admin@email.com", Password = "123", GivenName = "Jason", Surname = "Bryant", Role = "Administrator" },
            new UserModel() { MobilePhone = "09975722064", Username = "buyer", EmailAddress = "elyse.seller@email.com", Password = "123", GivenName = "Elyse", Surname = "Lambert", Role = "Buyer" },
        };
    }
}
