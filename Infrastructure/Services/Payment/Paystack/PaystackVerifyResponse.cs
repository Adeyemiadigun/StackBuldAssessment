using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Payment.Paystack
{
    public class PaystackVerifyResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = default!;
        public PaystackVerifyData Data { get; set; } = default!;
    }

    public class PaystackVerifyData
    {
        public string Status { get; set; } = default!;
        public string Reference { get; set; } = default!;
        public int Amount { get; set; }
        public string Gateway_Response { get; set; } = default!;
        public DateTime? Paid_At { get; set; }
    }
}
