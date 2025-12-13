using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Payment.Paystack
{
    public class PaystackInitResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PaystackInitData data { get; set; }
    }

    public class PaystackInitData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }

}
