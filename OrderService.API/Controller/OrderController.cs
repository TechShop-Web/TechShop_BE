using Microsoft.AspNetCore.Mvc;
using OrderService.Service.Interfaces;
using OrderService.Service.Models;

namespace OrderService.API.Controller
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            return Ok();
        }

        // GET: api/Order/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            return Ok();
        }

        // GET: api/Order/user/{userId}
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            return Ok();
        }

        // PUT: api/Order/{id}/cancel
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            return NoContent();
        }
    }
}
