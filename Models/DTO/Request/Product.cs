namespace ECommerceApi.Models.DTO.Request
{
    public class ProductReqDTO
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }
    }
}