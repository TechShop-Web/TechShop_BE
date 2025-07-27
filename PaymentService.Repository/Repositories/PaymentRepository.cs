using Microsoft.EntityFrameworkCore;
using PaymentService.Repository.Interfaces;
using PaymentService.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Repository.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }
        public async Task<Payment?> GetByOrderIdAsync(int orderId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

    }
}
