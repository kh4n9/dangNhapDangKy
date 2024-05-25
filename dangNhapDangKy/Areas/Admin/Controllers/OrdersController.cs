using Microsoft.AspNetCore.Mvc;
using dangNhapDangKy.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using dangNhapDangKy.Data;
using Microsoft.AspNetCore.Authorization;

namespace dangNhapDangKy.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, DateTime? startDate, DateTime? endDate, string searchString, string phoneNumber)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPhoneFilter"] = phoneNumber;
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            var orders = from o in _context.Orders
                         select o;

            // Áp dụng bộ lọc theo ngày
            if (startDate.HasValue && endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
            }

            // Áp dụng bộ lọc theo tên
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.FullName.Contains(searchString));
            }

            // Áp dụng bộ lọc theo số điện thoại
            if (!String.IsNullOrEmpty(phoneNumber))
            {
                orders = orders.Where(o => o.PhoneNumber.Contains(phoneNumber));
            }

            // Áp dụng bộ sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    orders = orders.OrderByDescending(o => o.FullName);
                    break;
                case "Date":
                    orders = orders.OrderBy(o => o.OrderDate);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
                default:
                    orders = orders.OrderBy(o => o.FullName);
                    break;
            }

            return View(await orders.AsNoTracking().ToListAsync());
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Ensure product details are included
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Address,PhoneNumber,TotalPrice,OrderDate,OrderStatus")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                                      .Include(o => o.OrderItems)
                                      .ThenInclude(oi => oi.Product)
                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Address,PhoneNumber,TotalPrice,OrderDate,OrderStatus,OrderItems")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
