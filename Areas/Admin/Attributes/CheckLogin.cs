//de ke thua class ActionFilterAttribute thi phai using dong ben duoi
using Microsoft.AspNetCore.Mvc.Filters;

namespace viettel_store.Areas.Admin.Attributes
{
    public class CheckLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //lấy giá trị của session email
            string email = context.HttpContext.Session.GetString("admin_email");
            if (string.IsNullOrEmpty(email))
            {
                context.HttpContext.Response.Redirect("/Admin/Account/Login");
            }
            base.OnActionExecuting(context);
        }
    }
}
