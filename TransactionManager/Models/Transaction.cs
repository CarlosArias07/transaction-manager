using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public float Amount { get; set; }
        public string PaymentStatus { get; set; }
    }
}
