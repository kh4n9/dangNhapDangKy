namespace dangNhapDangKy.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } // Id của người đặt hàng
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

}
