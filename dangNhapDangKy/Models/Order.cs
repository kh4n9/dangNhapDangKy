using System.ComponentModel.DataAnnotations;

namespace dangNhapDangKy.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public int OrderStatus { get; set; }
    }
}
