using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentService.API.Helpers;
using PaymentService.Repository.Interfaces;
using PaymentService.Repository.Models;
using PaymentService.Repository.Repositories;
using PaymentService.Service.BusinessModels;
using PaymentService.Service.Interfaces;


namespace PaymentService.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepository;

        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateVNPayPaymentUrlAsync(CreatePaymentRequest request, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)request.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toan hoa don cho Tech Shop");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public async Task<VnPayResponseModel> PaymentExecuteAsync(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            if (!response.Success) return response;

            var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.OrderId.ToString() == response.OrderId);

            if (payment != null)
            {
                payment.PaymentMethod = response.PaymentMethod;
                payment.PaymentStatus = "Success";
                payment.TransactionId = response.TransactionId;
                payment.ProcessedAt = DateTime.UtcNow;

                _paymentRepository.Update(payment);
                await _paymentRepository.SaveAsync();
            }

            return response;
        }
    }
}
