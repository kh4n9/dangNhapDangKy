using dangNhapDangKy.Data;
using dangNhapDangKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace dangNhapDangKy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include("Category").Include("Brand").ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize] // Yêu cầu người dùng đã đăng nhập
        public async Task<IActionResult> OrderHistory()
        {
            // Lấy thông tin về người dùng đã đăng nhập
            /*var user = await _userManager.GetUserAsync(User);*/
            string username = null;
            var user = new IdentityUser();
            if (User.Identity.IsAuthenticated)
            {
                username = User.Identity.Name;
                if (username != null) user = await _context.Users.FirstOrDefaultAsync(m => m.UserName == username);
            }

            if (user == null)
            {
                // Xử lý khi người dùng không tồn tại
                return NotFound();
            }

            // Lấy lịch sử mua hàng của người dùng từ cơ sở dữ liệu
            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return View(orders);
        }
        // Hiển thị sản phẩm theo loại sản phẩm
        public IActionResult ByCategory()
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View(products);
        }

        // hiển thị tất cả sản phẩm theo loại khi chọn trên dropdown
        public IActionResult FindOneCategory(int? id)
        {
            var products = _context.Products
                    .Where(p => p.CategoryId == id)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToList();
            ViewBag.Category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View(products);
        }

        // Hiển thị sản phẩm theo thương hiệu
        public IActionResult ByBrand()
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View(products);
        }

        // hiển thị tất cả sản phẩm theo thương hiệu khi chọn trên dropdown
        public IActionResult FindOneBrand(int? id)
        {
            var products = _context.Products
                    .Where(p => p.BrandId == id)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToList();
            ViewBag.Brand = _context.Brands.Where(b => b.Id == id).FirstOrDefault();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View(products);
        }
    }
}
