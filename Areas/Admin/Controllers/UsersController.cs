using Microsoft.AspNetCore.Mvc;

using viettel_store.Models; // nhìn thấy các file 
using BC = BCrypt.Net.BCrypt; // thư viện mã hóa
using X.PagedList; // thư viện phân trang

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {

        // khai báo đối tượng khai báo csdl
        public MyDbContext db = new MyDbContext();

        public IActionResult Index()
        {
            return Redirect("/Admin/Users/Read");
        }

        public IActionResult Read(int? page)
        {
            // Số bản ghi trên một trang
            int pageSize = 4;
            // số trang
            int pageNumber = page ?? 1;
            List<ItemUser> users = db.Users.OrderByDescending(item => item.Id).ToList();

            return View("Read", users.ToPagedList(pageNumber, pageSize));
        }

        // update
        public IActionResult Update(int id)
        {
            // tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Users/UpdatePost/" + id;
            // lấy 1 bản ghi tương ứng với id truyền vào
            ItemUser record = db.Users.FirstOrDefault(item => item.Id == id);
            return View("CreateUpdate", record);
        }

        // update sau khi ấn nút submit
        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString();
            // lấy 1 bản ghi tương ứng với id truyền vào
            ItemUser record = db.Users.FirstOrDefault(item => item.Id == id);
            if (record != null)
            {
                // kiểm tra xem email đã tồn tại chưa nếu chưa thì update
                ItemUser record_check = db.Users.FirstOrDefault(item => item.Id != id && item.Email == _Email);
                if (record_check == null)
                {
                    record.Name = _Name;
                    record.Email = _Email;
                    // nếu password k rỗng thì update 
                    if (!String.IsNullOrEmpty(_Password))
                    {
                        // mã hóa password
                        _Password = BC.HashPassword(_Password);
                        record.Password = _Password;
                    }
                    // cập nhật lại dữ liệu
                    db.Update(record);
                    db.SaveChanges();
                }
                else
                    return Redirect("/Admin/Users/Update/" + id + "?notify=email-exists");
            }

            return Redirect("/Admin/Users/Read");
        }

        
        // create
        public IActionResult Create()
        {
            // tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.acction = "/Admin/Users/CreatePost";
            return View("CreateUpdate");
        }

        // sau khi ấn submit
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString();
            // mã hóa password
            _Password = BC.HashPassword(_Password);
            // kiểm tra trong csdl đã tồn tại chưa nếu chưa thì update
            ItemUser record_check = db.Users.FirstOrDefault(item => item.Email == _Email);
            if (record_check == null)
            {
                ItemUser record = new ItemUser();
                record.Name = _Name;
                record.Email = _Email;
                record.Password = _Password;
                // cập nhật lại dữ liệu
                db.Users.Add(record);
                db.SaveChanges();
            }
            else
                return Redirect("/Admin/Users/Create?notify=email-exists");

            return Redirect("/Admin/Users/Read");
        }

        // delete
        public IActionResult Delete(int id)
        {
            // lấy 1 bản ghi tương ứng vs id truyền vào
            ItemUser record = db.Users.FirstOrDefault(item => item.Id == id);
            if(record != null)
            {
                db.Users.Remove(record);
                db.SaveChanges();
            }
            return Redirect("/Admin/Users/Read");
        }
    }
}
