namespace ECommerceApi.Models
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        public OrderModel Order { get; set; }
        public ProductModel Product { get; set; }
        public DateTime? ReceivedDate { get; set; }
    }
}