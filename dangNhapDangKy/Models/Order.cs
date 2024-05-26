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
        public int OrderStatus { get; set; } // 0 là đang xử lý 1 là đã xử lý 2 đã hoàn thành

        public String UserId { get; set; }
        public string GetOrderStatus()
        {
            return OrderStatus switch
            {
                0 => "Chờ xử lý",
                1 => "Đã xử lý",
                2 => "Đã hoàn thành",
                _ => "Không xác định"
            };
        }
    }
}
