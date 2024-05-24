namespace dangNhapDangKy.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(Product product, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == product.Id);
            if (item == null)
            {
                Items.Add(new CartItem { ProductId = product.Id, Product = product, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void UpdateItemQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(i => i.Product.Price * i.Quantity);
        }
    }
}
