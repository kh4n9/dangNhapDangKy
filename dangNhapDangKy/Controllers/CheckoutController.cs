using dangNhapDangKy.Data;
using dangNhapDangKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace dangNhapDangKy.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            if (cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var checkoutViewModel = new CheckoutViewModel();
            return View(checkoutViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cart = GetCart();
                if (cart.Items.Count == 0)
                {
                    ModelState.AddModelError("", "Your cart is empty.");
                    return View(model);
                }

                // Lưu đơn hàng vào cơ sở dữ liệu
                var order = new Order
                {
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    TotalPrice = cart.GetTotalPrice(),
                    OrderDate = DateTime.Now,
                    OrderStatus = 0,
                OrderItems = cart.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.Product.Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Clear the cart after successful checkout
                HttpContext.Session.Remove("Cart");

                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }

            return View(model);
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                                       .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
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
    }
}
