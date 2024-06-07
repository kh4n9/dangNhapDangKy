using dangNhapDangKy.Data;
using dangNhapDangKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace dangNhapDangKy.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
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
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            if (ModelState.IsValid)
            {
                var cart = GetCart();
                if (cart.Items.Count == 0)
                {
                    ModelState.AddModelError("", "Your cart is empty.");
                    return View(model);
                }

                string username = null;
                var user = new IdentityUser();
                if (User.Identity.IsAuthenticated)
                {
                    username = User.Identity.Name;
                    if (username != null) user = await _context.Users.FirstOrDefaultAsync(m => m.UserName == username);
                }

                // Lưu đơn hàng vào cơ sở dữ liệu
                var orderItems = new List<OrderItem>();
                foreach (var item in cart.Items)
                {
                    // Đảm bảo rằng Size đã được chọn
                    if (string.IsNullOrEmpty(item.Size))
                    {
                        ModelState.AddModelError("", "Please select a size for all items.");
                        return View(model);
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        Size = item.Size // Thêm kích thước vào OrderItem
                    };
                    orderItems.Add(orderItem);
                }

                var order = new Order
                {
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    TotalPrice = cart.GetTotalPrice(),
                    OrderDate = DateTime.Now,
                    OrderStatus = 0,
                    UserId = user.Id,
                    OrderItems = orderItems
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
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
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
