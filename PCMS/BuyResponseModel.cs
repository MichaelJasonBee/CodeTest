using System;
using System.Collections.Generic;
using System.Text;

namespace PCMS
{
    class BuyResponseModel
    {        
        public string Message { get; set; }
        public string NumberOfVouchersCreated { get; set; }
        public List<string> Vouchers { get; set; }
    }
}
