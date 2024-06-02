namespace dangNhapDangKy.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; } // Thêm thuộc tính Size
    }
}
