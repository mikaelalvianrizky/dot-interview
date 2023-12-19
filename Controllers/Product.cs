using ECommerceApi.Models;
using ECommerceApi.Models.DTO.Request;
using ECommerceApi.Models.DTO.Response.ResponseBuilders;
using ECommerceApi.Persistence;
using ECommerceApi.Services;
using ECommerceApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController: ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _memoryCache;
        public ProductController(DataContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<ProductModel> products;
            products = _memoryCache.Get<List<ProductModel>>("products");
            if(products is null) {
                products = await _context.Products.ToListAsync();
                _memoryCache.Set("products", products, TimeSpan.FromMinutes(1));
            }
            return Ok(ResponseBuilder.SuccessResponse("Success get list product", products));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            List<ProductModel> products = await _context.Products.Where(product => product.Id.Equals(id)).ToListAsync();
            
            if (products.Count != 0) return Ok(ResponseBuilder.SuccessResponse("Success get product", products[0]));
            return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Product not found"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductReqDTO productReq)
        {
            var validationResult = MinimalValidator.Validate(productReq);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Validation failed", validationResult.Errors));
            }
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
            var validationResult = MinimalValidator.Validate(productReq);
            if (!validationResult.IsValid)
            {
                return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Validation failed", validationResult.Errors));
            }
            ProductModel product = await _context.Products.Where(d => d.Id.Equals(id)).FirstOrDefaultAsync();

            if (product != null)
            {
                product.Name = productReq.Name;
                product.Brand = productReq.Brand;
                product.Price = productReq.Price;
                product.Size = productReq.Size;
                await _context.SaveChangesAsync();

                return Ok(ResponseBuilder.SuccessResponse("Success update product", product));
            }
            
            return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Product not found"));
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            ProductModel product = await _context.Products.Where(d => d.Id.Equals(id)).FirstOrDefaultAsync();
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(ResponseBuilder.SuccessResponse("Success delete product", product));
            }
            return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Product not found"));
        }
    }
}