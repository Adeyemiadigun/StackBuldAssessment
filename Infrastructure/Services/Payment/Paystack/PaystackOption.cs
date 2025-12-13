using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Payment.Paystack
{
    public class PaystackOptions
    {
        public string SecretKey { get; set; } = null!;
        public string BaseUrl { get; set; } = "https://api.paystack.co/";
    }

}
