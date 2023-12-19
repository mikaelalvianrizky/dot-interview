namespace ECommerceApi.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public UserModel User { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}