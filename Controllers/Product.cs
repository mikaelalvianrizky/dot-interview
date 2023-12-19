using ECommerceApi.Models;
using ECommerceApi.Models.DTO.Request;
using ECommerceApi.Models.DTO.Response.ResponseBuilders;
using ECommerceApi.Persistence;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController: ControllerBase
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<ProductModel> products = await _context.Products.ToListAsync();
            return Ok(ResponseBuilder.SuccessResponse("Success get list product", products));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            List<ProductModel> products = await _context.Products.Where(product => product.Id.Equals(id)).ToListAsync();
            
            if (products.Count != 0) return Ok(ResponseBuilder.SuccessResponse("Success get product", products[0]));
            return BadRequest(ResponseBuilder.ErrorResponse(400, "Product not found"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductReqDTO productReq)
        {
            ProductModel product = new()
            {
                Name = productReq.Name,
                Brand = productReq.Brand,
                Price = productReq.Price,
                Size = productReq.Size
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(ResponseBuilder.SuccessResponse("Success create product", product));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductReqDTO productReq)
        {
            ProductModel product = await _context.Products.Where(d => d.Id.Equals(id)).FirstAsync();

            if (product != null)
            {
                product.Name = productReq.Name;
                product.Brand = productReq.Brand;
                product.Price = productReq.Price;
                product.Size = productReq.Size;
                await _context.SaveChangesAsync();

                return Ok(ResponseBuilder.SuccessResponse("Success update product", product));
            }
            
            return BadRequest(ResponseBuilder.ErrorResponse(400, "Product not found"));
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            ProductModel product = await _context.Products.Where(d => d.Id.Equals(id)).FirstAsync();

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(ResponseBuilder.SuccessResponse("Success update product", product));
            }
            return BadRequest(ResponseBuilder.ErrorResponse(400, "Product not found"));
        }
    }
}