namespace dangNhapDangKy.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Id của người dùng nếu cần
        public ICollection<CartItem> CartItems { get; set; }
        public int TotalItems => CartItems?.Sum(item => item.Quantity) ?? 0;
        public decimal TotalPrice => CartItems?.Sum(item => item.Quantity * item.Price) ?? 0;
    }
}
