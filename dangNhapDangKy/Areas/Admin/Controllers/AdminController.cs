using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dangNhapDangKy.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "Admin")]
        [Area("Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
