using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaymentService.API.Helpers;
using PaymentService.Repository.Interfaces;
using PaymentService.Repository.Models;
using PaymentService.Service.BusinessModels;
using PaymentService.Service.Interfaces;


namespace PaymentService.Service.Services
{
    public class PaymentService : IPaymentService
    {

        private readonly IPaymentRepository _paymentRepository;

        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration, IPaymentRepository paymentRepository)
        {
            _configuration = configuration;
            _paymentRepository = paymentRepository;
        }

        public async Task<string> CreateVNPayPaymentUrlAsync(CreatePaymentRequest request, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

            var pay = new VnPayLibrary();
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)request.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan hoa don cho Tech Shop");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", request.OrderId.ToString());

            var payment = new Payment
            {
                //OrderId = request.OrderId,
                PaymentMethod = (Repository.Enums.PaymentMethod)request.PaymentMethod,
                PaymentStatus = Repository.Enums.PaymentStatus.PENDING,
                TransactionId = null,
                Amount = request.Amount,
                CreatedAt = timeNow,
                ProcessedAt = DateTime.MinValue
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveAsync();

            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }


        public async Task<VnPayResponseModel> PaymentExecuteAsync(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            if (!response.Success) return response;


            //var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.OrderId.ToString() == response.OrderId);

            //if (payment != null)
            //{
            //    payment.PaymentMethod = response.PaymentMethod;
            //    payment.PaymentStatus = "Success";
            //    payment.TransactionId = response.TransactionId;
            //    payment.ProcessedAt = DateTime.UtcNow;

            //    _paymentRepository.Update(payment);
            //    await _paymentRepository.SaveAsync();
            //}

            return response;
        }

    }
}
