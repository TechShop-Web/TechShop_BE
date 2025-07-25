using PaymentService.Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace PaymentService.Service.Interfaces
{
    public interface IPaymentService
    {
        string CreateVNPayPaymentUrlAsync(CreatePaymentRequest request, HttpContext context);
        Task<VnPayResponseModel> PaymentExecuteAsync(IQueryCollection collections);
        //Task<bool> HandleVNPayReturnAsync(IQueryCollection vnpData);
    }
}
