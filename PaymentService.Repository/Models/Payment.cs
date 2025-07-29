using PaymentService.Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Repository.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TransactionId { get; set; }
        public float Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
