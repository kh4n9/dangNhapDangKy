using dangNhapDangKy.Data;
using dangNhapDangKy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace dangNhapDangKy.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddItem(product, quantity);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveItem(productId);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult UpdateCartItemQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            cart.UpdateItemQuantity(productId, quantity);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        private Cart GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(sessionCart))
            {
                return new Cart();
            }
            return JsonConvert.DeserializeObject<Cart>(sessionCart);
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
    }
}
