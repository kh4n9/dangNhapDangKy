using dangNhapDangKy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dangNhapDangKy.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Dashboard()
        {
            // Assume you have methods to get the necessary data
            var monthlyEarnings = GetMonthlyEarnings();
            var mostPurchasedProduct = GetMostPurchasedProduct();
            var pendingOrdersCount = GetPendingOrdersCount();
            var customerWithHighestPurchase = GetCustomerWithHighestPurchase();

            // Pass data to the view using ViewBag or ViewData
            ViewBag.MonthlyEarnings = monthlyEarnings;
            ViewBag.MostPurchasedProduct = mostPurchasedProduct;
            ViewBag.PendingOrdersCount = pendingOrdersCount;
            ViewBag.CustomerWithHighestPurchase = customerWithHighestPurchase;
            ViewBag.MonthlyEarningsByMonth = GetMonthlyEarningsByMonth();

            return View();
        }

        // Define methods to get data from your data source
        private decimal GetMonthlyEarnings()
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31).AddDays(1).AddSeconds(-1);

            // Query the database directly using Entity Framework Core
            var totalMonthlyEarnings = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Sum(o => o.TotalPrice);

            return totalMonthlyEarnings;
        }

        private string GetMostPurchasedProduct()
        {
            // Query the database to get the product with the highest total quantity across all orders
            var mostPurchasedProduct = _context.OrderItem
                .GroupBy(oi => oi.ProductId) // Group by ProductId
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity) // Calculate total quantity for each product
                })
                .OrderByDescending(g => g.TotalQuantity) // Order by total quantity in descending order
                .FirstOrDefault(); // Take the first product

            if (mostPurchasedProduct != null)
            {
                // Retrieve the product name based on its ID
                var productName = _context.Products
                    .Where(p => p.Id == mostPurchasedProduct.ProductId)
                    .Select(p => p.Name)
                    .FirstOrDefault();

                return productName ?? "Unknown"; // Return the product name or "Unknown" if not found
            }

            return "Unknown"; // Return "Unknown" if there are no order items
        }


        private int GetPendingOrdersCount()
        {
            // Query the orders where the order status is not yet completed (assuming 0 means pending status)
            var pendingOrdersCount = _context.Orders.Count(o => o.OrderStatus != 2); // Assuming 2 means completed status

            return pendingOrdersCount;
        }


        private string GetCustomerWithHighestPurchase()
        {
            // Truy vấn cơ sở dữ liệu để lấy tổng số tiền mỗi khách hàng đã mua
            var customerPurchases = _context.Orders
                                            .GroupBy(o => o.UserId)
                                            .Select(g => new
                                            {
                                                CustomerId = g.Key,
                                                TotalPurchase = g.Sum(o => o.TotalPrice)
                                            })
                                            .ToList();

            // Tìm ra khách hàng có tổng số tiền mua sản phẩm cao nhất
            var customerWithHighestPurchase = customerPurchases.OrderByDescending(cp => cp.TotalPurchase).FirstOrDefault();

            // Kiểm tra nếu có khách hàng nào
            if (customerWithHighestPurchase != null)
            {
                // Lấy thông tin của khách hàng từ cơ sở dữ liệu (ví dụ: tên khách hàng)
                var customer = _context.Users.FirstOrDefault(u => u.Id == customerWithHighestPurchase.CustomerId);

                // Trả về tên của khách hàng có tổng số tiền mua sản phẩm cao nhất
                if (customer != null)
                {
                    return customer.UserName; // Đây là tên của khách hàng
                }
            }

            // Trả về giá trị mặc định nếu không tìm thấy khách hàng nào
            return "Không có khách hàng";
        }
        private List<decimal> GetMonthlyEarningsByMonth()
        {
            // Khởi tạo danh sách để lưu trữ doanh thu của từng tháng
            List<decimal> monthlyEarningsByMonth = new List<decimal>();

            // Lặp qua từng tháng trong năm
            for (int month = 1; month <= 12; month++)
            {
                // Lấy ngày đầu tiên của tháng
                DateTime startDate = new DateTime(DateTime.Now.Year, month, 1);

                // Lấy ngày cuối cùng của tháng
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                // Tính tổng doanh thu từ các đơn hàng trong khoảng thời gian từ startDate đến endDate
                decimal totalMonthlyEarnings = _context.Orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .Sum(o => o.TotalPrice);

                // Thêm tổng doanh thu của tháng vào danh sách
                monthlyEarningsByMonth.Add(totalMonthlyEarnings);
            }

            return monthlyEarningsByMonth;
        }


    }
}
