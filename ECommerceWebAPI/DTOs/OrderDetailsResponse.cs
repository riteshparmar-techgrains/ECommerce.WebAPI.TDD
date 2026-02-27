namespace ECommerceWebAPI.DTOs
{
    public class OrderDetailsResponse
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
