using System.Drawing.Drawing2D;
using System.Drawing;

namespace dangNhapDangKy.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Size> Sizes { get; set; }
        public ICollection<Image> Images { get; set; }
    }

}
