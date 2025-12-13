using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class PaymentTransaction : BaseClass
    {
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Amount { get; private set; }
        public string Provider { get; private set; } = null!;
        public string Reference { get; private set; } = null!;
        public PaymentStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private PaymentTransaction() { }

        public PaymentTransaction(
            Guid orderId,
            Guid userId,
            decimal amount,
            string provider,
            string reference)
        {
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            Provider = provider;
            Reference = reference;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkSuccessful() => Status = PaymentStatus.Success;
        public void MarkFailed() => Status = PaymentStatus.Failed;
    }

}
