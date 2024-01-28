using Microsoft.AspNetCore.Mvc;
using viettel_store.Models;
using X.PagedList;
namespace viettel_store.Controllers
{
    public class NewsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        // Detail
        public IActionResult Detail(int id)
        {
            var record = db.News.Where(item => item.Id == id).FirstOrDefault();

            return View("NewsDetail", record);
        }
    }
}
