using ECommerceApi.Models.DTO.Response.ResponseBuilders;
using ECommerceApi.Persistence;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController: ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(DataContext context)
        {
            _productService = new ProductService(context);
        }

        [HttpGet]
        [Route("product")]
        public IActionResult GetList()
        {
            return Ok(ResponseBuilder.SuccessResponse("Success get list product", _productService.GetProducts()));
        }
    }
}