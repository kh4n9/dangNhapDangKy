using dangNhapDangKy.Data;
using dangNhapDangKy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dangNhapDangKy.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cart = GetCart();
            foreach (var item in cart.Items)
            {
                var product = await _context.Products
                    .Include(p => p.Sizes)
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product != null)
                {
                    item.Product = product; // Assign the product to the cart item
                }
            }
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

        // POST: UpdateCartItemSize
        [HttpPost]
        public async Task<IActionResult> UpdateCartItemSize(int productId, string size)
        {
            var cart = GetCart();
            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Size = size;
                SaveCart(cart);
            }
            return RedirectToAction(nameof(Index));
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
