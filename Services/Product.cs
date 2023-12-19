using ECommerceApi.Models;
using ECommerceApi.Models.DTO.Request;
using ECommerceApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class ProductService
    {
        private readonly DataContext _context;
        public ProductService(DataContext context)
        {
            _context = context;
        }
        
        public async Task<List<ProductModel>> GetProducts()
        {
            List<ProductModel> products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task<ProductModel> GetProductById(int id)
        {
            ProductModel product = await _context.Products.Where(product => product.Id.Equals(id)).FirstOrDefaultAsync();
            return product;
        }

        public async Task<ProductModel> CreateProduct(ProductReqDTO productReq)
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

            return product;
        }
    }
}