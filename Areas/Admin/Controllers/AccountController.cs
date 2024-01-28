using Microsoft.AspNetCore.Mvc;
using viettel_store.Models;
using BC = BCrypt.Net.BCrypt; // mã hóa password

namespace viettel_store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        public MyDbContext db = new MyDbContext();

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();  
        }

        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc)
        {
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            // lấy 1 bản ghi tương ứng với email để truyền vào 
            var record = db.Users.Where(item => item.Email == _Email).FirstOrDefault();
            if (record != null)
            {
                if (BC.Verify(_Password, record.Password))
                {
                    //khoi tao session UserId
                    HttpContext.Session.SetString("admin_user_id", record.Id.ToString());
                    //khoi tao session Email
                    HttpContext.Session.SetString("admin_email", _Email);
                    //di chuyen den url
                    return Redirect("/Admin/Home");
                }
            }
            return Redirect("/Admin/Account/Login");
        }

        // logout
        public IActionResult Logout()
        {
            // Xóa các session đã đặt lúc login thành công
            HttpContext.Session.Remove("admin_user_id");
            HttpContext.Session.Remove("admin_email");
            return Redirect("/Admin/Account/Login");
        }
    }
}
