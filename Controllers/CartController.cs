using Microsoft.AspNetCore.Mvc;
using viettel_store.Models;
using Newtonsoft.Json;

namespace viettel_store.Controllers
{
	public class CartController : Controller
	{
		public MyDbContext db = new MyDbContext();

		// hiển thị danh sách các sản phẩm trong giỏ hàng
		public IActionResult Index()
		{
			List<Item> shopping_cart = new List<Item>();
			string json_cart = HttpContext.Session.GetString("cart");
			if(!String.IsNullOrEmpty(json_cart))
			{
				// convert chuỗi json
				shopping_cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);

			}	
			return View(shopping_cart);
		}

		// thêm sản phẩm vào giỏ hàng
		public IActionResult Buy(int id)
		{
			// gọi hàm CartAdd (trong class Cart trong Models/Cart.cs) để thêm sản phẩm
			Cart.CartAdd(HttpContext.Session, id);
			// di chuyển đến action Index
			return RedirectToAction("Index");
		}

		// xóa sản phẩm khỏi giỏ hàng
		public IActionResult Remove(int id)
		{
			Cart.CartRemove(HttpContext.Session, id);
			// di chuyển đến action Index
			return RedirectToAction("Index");
		}

		// xóa toàn bộ sản phẩm trong giỏ hàng
		public IActionResult Destroy()
		{
			Cart.CartDestroy(HttpContext.Session);
			// di chuyển action đến Index
			return Redirect("/Cart/Index?notify=destroy-success");
		}

		// thanh toán đơn hàng thông qua hàm CheckOut
		public IActionResult CheckOut()
		{
			// nếu user chưa đăng nhập thì di chuyển đến url đăng nhập
			if(String.IsNullOrEmpty(HttpContext.Session.GetString("customer_id")))
			{
				return Redirect("/Account/Login");
			}	
			else
			{
				// lấy id của customer
				int customer_id = Convert.ToInt32(HttpContext.Session.GetString("customer_id"));
				Cart.CartCheckOut(HttpContext.Session, customer_id);
				
			}
			// di chuyển đến action Index
			return Redirect("/Cart/Index?notify=checkout-success");
		}

		// update số lượng sản phẩm
		public IActionResult Update()
		{
			List<Item> shopping_cart = new List<Item>();
			string json_cart = HttpContext.Session.GetString("cart");
			if(!String.IsNullOrEmpty(json_cart))
			{
				// convert chuỗi json
				shopping_cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
			}	
			// duyệt các phần tử
			foreach(var cart_item in shopping_cart)
			{
				// lấy số lượng sản phẩm tử thẻ input
				int new_quantity = Convert.ToInt32(Request.Form["product_" + cart_item.ProductRecord.Id]);
				// gọi hàm cập nhật lại số lượng sản phẩm
				Cart.CartUpdate(HttpContext.Session, cart_item.ProductRecord.Id, new_quantity);
			}	
			// di chuyển đến action Index
			return RedirectToAction("Index");
		}
	}
}
