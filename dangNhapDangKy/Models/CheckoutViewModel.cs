using System.ComponentModel.DataAnnotations;

namespace dangNhapDangKy.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
