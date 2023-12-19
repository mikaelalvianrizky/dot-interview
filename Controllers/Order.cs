using ECommerceApi.Models;
using ECommerceApi.Models.DTO.Request;
using ECommerceApi.Models.DTO.Response.ResponseBuilders;
using ECommerceApi.Persistence;
using ECommerceApi.Services;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApi.Models.DTO.Response;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController: ControllerBase
    {
        private readonly DataContext _context;
        public OrderController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var bearer = Request.Headers.Authorization;
            var jwt = bearer.ToString()[7..];
            var token = new JwtSecurityToken(jwt);

            List<OrderDetailModel> orderDetail = await _context.OrderDetails.Where(
                od => od.Order.User.Id.ToString() == (string)token.Payload["Id"]
            ).Include(od => od.Product).Include(od => od.Order).ToListAsync();

            return Ok(ResponseBuilder.SuccessResponse("Success get list order", orderDetail));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bearer = Request.Headers.Authorization;
            var jwt = bearer.ToString()[7..];
            var token = new JwtSecurityToken(jwt);
            
            List<OrderDetailModel> orderDetail = await _context.OrderDetails.Where(
                od => od.Order.Id.Equals(id) && od.Order.User.Id.ToString() == (string)token.Payload["Id"]
            ).Include(od => od.Product).Include(od => od.Order).ToListAsync();

            if (orderDetail.Count == 0) return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Order not found"));

            return Ok(ResponseBuilder.SuccessResponse("Success get order", orderDetail));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderReqDTO orderReq)
        {
            var bearer = Request.Headers.Authorization;
            var jwt = bearer.ToString()[7..];
            var token = new JwtSecurityToken(jwt);
            UserModel user = await _context.Users.Where(user => user.Email == (string) token.Payload["Email"]).FirstOrDefaultAsync();

            List<string> errors = new List<string>();
            OrderModel order = new()
            {
                Name = orderReq.Name,
                Address = orderReq.Address,
                Phone = orderReq.Phone,
                User = user
            };

            foreach (int productId in orderReq.ProductIds)
            {
                ProductModel product = await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
                if (product == null) 
                {
                    errors.Add("product " + productId + " not found");
                    continue;
                }
                OrderDetailModel orderDetail = new()
                {
                    Order = order,
                    Product = product
                };
                _context.OrderDetails.Add(orderDetail);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(ResponseBuilder.SuccessResponse("Success create order", order));
        }

        [HttpPut]
        [Route("{order_detail_id}")]
        public async Task<IActionResult> UpdateReceivedDate(int order_detail_id)
        {
            OrderDetailModel orderDetail = await _context.OrderDetails.Where(d => d.Id.Equals(order_detail_id)).FirstOrDefaultAsync();

            if (orderDetail != null)
            {
                orderDetail.ReceivedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(ResponseBuilder.SuccessResponse("Success update orderDetail", orderDetail));
            }

            return BadRequest(ResponseBuilder.ErrorResponse(400, "Product not found"));
        }
    }
}