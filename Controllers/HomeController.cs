using Microsoft.AspNetCore.Mvc;

namespace viettel_store.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
            // di chuyển đến trang Admin
            //return Redirect("/Admin");
        }
    }
}
