using AgriEcommerce.Models;
using System;
using System.Linq;
using System.Web.Mvc;

public class TaiKhoanController : Controller
{
    private AgriEcommerceEntities db = new AgriEcommerceEntities();

    public ActionResult DangNhap(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DangNhap(NguoiDung objUser, string returnUrl)
    {
        if (string.IsNullOrEmpty(objUser.Email))
        {
            ViewBag.ThongBao = "Email không được để trống!";
            return View(objUser);
        }

        if (string.IsNullOrEmpty(objUser.MatKhau))
        {
            ViewBag.ThongBao = "Mật khẩu không được để trống!";
            return View(objUser);
        }

        NguoiDungBusiness nguoiDungBusiness = new NguoiDungBusiness();
        NguoiDung objUserdb = nguoiDungBusiness.KiemTraDangNhap(objUser.Email);

        if (objUserdb == null)
        {
            ViewBag.ThongBao = "Tài khoản không tồn tại!";
            return View(objUser);
        }

        string matKhauMaHoa = Common.ToMD5(objUser.MatKhau);

        if (objUserdb.MatKhau != matKhauMaHoa)
        {
            ViewBag.ThongBao = "Mật khẩu không đúng!";
            return View(objUser);
        }

        Session["UserOnline"] = objUserdb;

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("danhsach", "TrangChu");
    }

    // Đăng xuất
    public ActionResult DangXuat()
    {
        Session.Clear();
        Session.Abandon();
        return RedirectToAction("danhsach", "TrangChu");
    }

    public ActionResult ThongTin()
    {
        if (Session["UserOnline"] == null)
        {
            return RedirectToAction("DangNhap");
        }

        NguoiDung userSession = (NguoiDung)Session["UserOnline"];

        var user = db.NguoiDungs.Find(userSession.MaNguoiDung);

        return View(user);
    }

    public ActionResult DangKy()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DangKy(NguoiDung user, string NhapLaiMatKhau)
    {
        // kiểm tra nhập đầy đủ
        if (string.IsNullOrEmpty(user.HoTen) ||
            string.IsNullOrEmpty(user.Email) ||
            string.IsNullOrEmpty(user.DienThoai) ||
            string.IsNullOrEmpty(user.DiaChi) ||
            string.IsNullOrEmpty(user.MatKhau) ||
            string.IsNullOrEmpty(NhapLaiMatKhau))
        {
            ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";
            return View(user);
        }

        // kiểm tra email tồn tại
        var checkEmail = db.NguoiDungs.FirstOrDefault(x => x.Email == user.Email);

        if (checkEmail != null)
        {
            ViewBag.ThongBao = "Email đã tồn tại!";
            return View(user);
        }

        // kiểm tra mật khẩu nhập lại
        if (user.MatKhau != NhapLaiMatKhau)
        {
            ViewBag.ThongBao = "Mật khẩu nhập lại không khớp!";
            return View(user);
        }

        try
        {
            user.MaNguoiDung = Guid.NewGuid().ToString("N").Substring(0, 10);

            user.MatKhau = Common.ToMD5(user.MatKhau);

            user.MaVaiTro = "VT02";

            user.NgayTao = DateTime.Now;

            db.NguoiDungs.Add(user);
            db.SaveChanges();

            TempData["DangKyThanhCong"] = "Đăng ký tài khoản thành công!";
            return RedirectToAction("DangNhap");
        }
        catch
        {
            ViewBag.ThongBao = "Có lỗi xảy ra khi đăng ký!";
            return View(user);
        }
    }

    public ActionResult QuenMatKhau()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SuaThongTin(NguoiDung model)
    {
        var user = db.NguoiDungs.Find(model.MaNguoiDung);

        if (user != null)
        {
            user.HoTen = model.HoTen;
            user.DienThoai = model.DienThoai;
            user.DiaChi = model.DiaChi;

            db.SaveChanges();

            Session["UserOnline"] = user;

            return RedirectToAction("ThongTin");
        }

        return View(model);
    }

}