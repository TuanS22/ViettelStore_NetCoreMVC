using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using viettel_store.Areas.Admin.Attributes;

namespace viettel_store.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        // kiểm tra đăng nhập
        [CheckLogin]
        public IActionResult Index()
        {
            return View();
        }
    }
}
