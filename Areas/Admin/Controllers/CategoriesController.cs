using Microsoft.AspNetCore.Mvc;
using viettel_store.Models;
using X.PagedList;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using viettel_store.Areas.Admin.Attributes;


namespace viettel_store.Areas.Admin.Controllers
{
    [Area("Admin")]
    // kiểm tra đăng nhập
    [CheckLogin]
    public class CategoriesController : Controller
    {
        // tạo biến lưu chuỗi kết nối
        string strConnectionString;
        // định nghĩa hàm tạo: là hàm sẽ tự động được triệu gọi khi class này hoạt động
        public CategoriesController()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            this.strConnectionString = config.GetConnectionString("MyConnectionString").ToString();
        }

        public IActionResult Index()
        {
            return Redirect("/Admin/Categories/Read");
        }

        public IActionResult Read(int? page)
        {
            // sử dụng ADO để truyền câu truy vấn sql và lấy kết quả trả về
            // tạo đối tượng DataTable để đổ dữ liệu từ kết quả truy vấn
            DataTable dtCategories = new DataTable();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                // tạo đối tượng DataAdapter để truyền câu lệnh sql và lấy kết quả trả về
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where ParentId = 0 order by Id desc", conn);
                // Fill dữ liệu từ da vào DataTable
                da.Fill(dtCategories);
            }

            //--
            //do thư viện X.PagedList sử dụng List để phân trang, vì vậy cần chuyển đổi DataTable có tên là dtCategories sang List
            // tạo đối tượng List để lưu trữ dữ liệu từ dtCategories chuyển sang
            List<ItemCategory> listCategory = new List<ItemCategory>();
            // duyệt các row của dtCategories và tạo các row List để add vào List
            foreach (DataRow item in dtCategories.Rows)
            {
                listCategory.Add(new ItemCategory() { Id = Convert.ToInt32(item["id"]), ParentId = Convert.ToInt32(item["ParentId"]), Name = item["Name"].ToString(), DisplayHomePage = Convert.ToInt32(item["DisplayHomePage"]) });
            }
            //--
            // Sử dụng X.PageList để phân trang
            int pageSize = 4;
            int pageNumber = page ?? 1;
            return View("Read", listCategory.ToPagedList(pageNumber, pageSize));
        }

        // update
        public IActionResult Update(int id)
        {
            // tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/UpdatePost/" + id;
            DataTable dtCategory = new DataTable();

            // lấy 1 bản ghi tương ứng vs id để truyền vào
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where Id = " + id, conn);
                // Fill dữ liệu từ da vào DataTable
                da.Fill(dtCategory);
            }
            // bản ghi của dtCategory là 1 table chứa 1 row (do truy vấn ở trên chỉ trả về 1 row)
            return View("CreateUpdate", dtCategory);
        }

        [HttpPost]
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            // sử dụng đối tượng IFormCollection để lấy dữ liệu theo kiểu POST
            string _Name = fc["Name"].ToString().Trim();
            // sử dụng đối tượng Request để lấy dữ liệu theo kiểu POST
            int _ParentId = Convert.ToInt32(Request.Form["ParentId"]);
            int _DisplayHomePage = !String.IsNullOrEmpty(Request.Form["DisplayHomePage"]) ? 1 : 0;
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                // chú ý: phải có dòng dưới này
                conn.Open();
                SqlCommand cmd = new SqlCommand("update Categories set Name=@var_name, ParentId=@var_parent_id, DisplayHomePage=@var_display_home_page where Id=@id", conn);
                // truyền các giá trị vào chuỗi truy vấn
                cmd.Parameters.AddWithValue("@var_name", _Name);
                cmd.Parameters.AddWithValue("@var_parent_id", _ParentId);
                cmd.Parameters.AddWithValue("@var_display_home_page", _DisplayHomePage);
                cmd.Parameters.AddWithValue("@id", id);
                // thực hiện truy vấn
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories/Read");
        }

        // create
        public IActionResult Create()
        {
            // tạo biến action để đưa vào thuộc tính action của thẻ form
            ViewBag.action = "/Admin/Categories/CreatePost";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            // sử dụng đối tượng IFormCollection để lấy dữu liệu theo kiểu POST
            string _Name = fc["Name"].ToString().Trim();
            // sử dụng đối tượng Request để lấy
            int _ParentId = Convert.ToInt32(Request.Form["ParentId"]);
            int _DisplayHomePage = !String.IsNullOrEmpty(Request.Form["DisplayHomePage"]) ? 1 : 0;
            using(SqlConnection conn = new SqlConnection(strConnectionString))
            {
                // phải có Open()
                conn.Open();
                // sử dụng Command để truy vấn dữ liệu
                SqlCommand cmd = new SqlCommand("insert into Categories(Name, ParentId, DisplayHomePage) values(@var_name, @var_parent_id, @var_display_home_page)", conn);
                // truyền các giá trị vào chuỗi truy vấn
                cmd.Parameters.AddWithValue("@var_name", _Name);
                cmd.Parameters.AddWithValue("@var_parent_id", _ParentId);
                cmd.Parameters.AddWithValue("@var_display_home_page", _DisplayHomePage);
                // thực hiện truy vấn
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories/Read");
        }

         // delete
         public IActionResult Delete(int id)
        {
            using(SqlConnection conn = new SqlConnection(strConnectionString))
            {
                // mở kết nối
                conn.Open();
                // truyền các giá trị vào chuỗi vào truy vấn
                SqlCommand cmd = new SqlCommand("delete from Categories where Id = @id or ParentId = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                // thực hiện câu lệnh
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories/Read");
        }
    }
}
