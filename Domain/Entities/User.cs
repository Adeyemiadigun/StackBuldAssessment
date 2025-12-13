using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class User : BaseClass
    {
        public string Email { get;private set; }
        public string PasswordHash { get; private set; }
        public string PasswordSalt { get;private  set; }
        public DateTime CreatedAt { get; private set; }
        public UserRole Role { get; private set; }
        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders;

        private readonly List<PaymentTransaction> _transactions = new();
        public IReadOnlyCollection<PaymentTransaction> Transactions => _transactions;
        private User() { } // EF

        public User(string email, string passwordHash, string salt, UserRole role)
        {
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            PasswordSalt = salt;
            Role = role;
        }
    }
}
