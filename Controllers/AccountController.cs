using Microsoft.AspNetCore.Mvc;
using viettel_store.Models;
using X.PagedList;
using BC = BCrypt.Net.BCrypt; // mã hóa password

namespace viettel_store.Controllers
{
    public class AccountController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return View();
        }

        // đăng nhập
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc)
        {
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            //lay mot ban ghi tuong ung voi email truyen vao
            var record = db.Customers.Where(item => item.Email == _Email).FirstOrDefault();
            if (record != null)
            {
                if (BC.Verify(_Password, record.Password))
                {
                    //khoi tao session UserId
                    HttpContext.Session.SetString("customer_id", record.Id.ToString());
                    //khoi tao session Email
                    HttpContext.Session.SetString("customer_email", _Email);
                    //di chuyen den url
                    return Redirect("/Home");
                }
                else
                    return Redirect("/Account/Login?notify=invalid");
            }
            else
                return Redirect("/Account/Login?notify=invalid");
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            // Xóa session
            HttpContext.Session.Remove("customer_email");
            HttpContext.Session.Remove("customer_id");
            return Redirect("/Home");
        }

        // đăng ký
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterPost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Email = fc["Email"].ToString().Trim();
            string _Phone = fc["Phone"].ToString().Trim();
            string _Address = fc["Address"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            // mã hóa password
            _Password = BC.HashPassword(_Password);
            // kiểm tra trong csdl đã tồn tại chưa nếu chưa thì update
            ItemCustomer record_check = db.Customers.FirstOrDefault(item => item.Email == _Email);
            if (record_check == null)
            {
                ItemCustomer record = new ItemCustomer();
                record.Name = _Name;
                record.Email = _Email;
                record.Password = _Password;
                record.Phone = _Phone;
                record.Address = _Address;
                // cập nhật lại dữ liệu
                db.Customers.Add(record);
                db.SaveChanges();
            }
            else
                return Redirect("/Account/Register?notify=email-exists");
            return Redirect("/Account/Register?notify=success");
        }

        // danh sách đơn hàng
        public IActionResult Orders(int? page)
        {
            // lấy CustomerId để hiện thị các đơn hàng của người này
            int customer_id = 0;
            
             if (String.IsNullOrEmpty(HttpContext.Session.GetString("customer_id")))
                return Redirect("/Account/Login");
            else
            customer_id = Convert.ToInt32(HttpContext.Session.GetString("customer_id"));
            //
            int _CurrentPage = page ?? 1;
            //dinh nghia so ban ghi tren mot trang
            int _RecordPerPage = 20;

            //lay tat ca cac ban ghi trong table News
            List<ItemOrder> listRecord = db.Orders.Where(item => item.CustomerId == customer_id).OrderByDescending(item => item.Id).ToList();
            //truyen gia tri ra view co phan trang
            //return Content(HttpContext.Session.GetString("id"));
            return View("Orders", listRecord.ToPagedList(_CurrentPage, _RecordPerPage));
        }

        // chi tiết sản phẩm
        public IActionResult Detail(int? id)
        {
            int _OrderId = id ?? 0;
            ViewBag.OrderId = _OrderId;
            // lấy danh sách các sản phẩm thuộc đơn hàng
            List<ItemOrderDetail> _ListRecord = db.OrdersDetail.Where(item => item.OrderId == _OrderId).ToList();
            return View("Detail", _ListRecord);
        }   
        
        // hủy đơn hàng
        public IActionResult Cancel(int? id)
        {
            int _OrderId = id ?? 0;
            ItemOrder record = db.Orders.Where(item => item.Id == _OrderId).FirstOrDefault();
            if(record != null)
            {
                record.Status = 2; // Status = 2 tương ứng vs hủy đơn hàng
                // cập nhật lại kêt quả
                db.SaveChanges();
            }
            return Redirect("/Account/Orders");
        }
    }
}
