using AgriEcommerce.Models;
using System.Web.Mvc;
using System.Web.Routing;

namespace AgriEcommerce.Controllers
{
    public class AdminBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = Session["UserOnline"];

            // Chưa đăng nhập
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "TaiKhoan", action = "DangNhap" }
                    ));
                return;
            }

            // Kiểm tra quyền Admin
            NguoiDung user = (NguoiDung)session;

            if (user.MaVaiTro != "VT01") // VT01 = Admin
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "TrangChu", action = "danhsach" }
                    ));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}