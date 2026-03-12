using AgriEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class GioHangController : Controller
    {
        private AgriEcommerceEntities db = new AgriEcommerceEntities();

        // Hiển thị giỏ hàng
        public ActionResult DanhSach()
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            var gioHang = db.GioHangs.FirstOrDefault(x => x.MaNguoiDung == user.MaNguoiDung);

            if (gioHang == null)
                return View(new List<GioHangChiTiet>());

            var chiTiet = db.GioHangChiTiets.Include("SanPham").Where(x => x.MaGioHang == gioHang.MaGioHang).ToList();

            return View(chiTiet);
        }

        // Thêm sản phẩm vào giỏ
        public ActionResult ThemGioHang(string id)
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var gioHang = db.GioHangs.FirstOrDefault(x => x.MaNguoiDung == user.MaNguoiDung);

            // Nếu chưa có giỏ hàng thì tạo mới
            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    MaGioHang = Guid.NewGuid().ToString("N").Substring(0, 20),
                    MaNguoiDung = user.MaNguoiDung,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                db.GioHangs.Add(gioHang);
                db.SaveChanges();
            }

            var sanPham = db.SanPhams.Find(id);

            if (sanPham == null)
                return HttpNotFound();

            var chiTiet = db.GioHangChiTiets.FirstOrDefault(x => x.MaGioHang == gioHang.MaGioHang && x.MaSanPham == id);

            if (chiTiet != null)
            {
                chiTiet.SoLuong = (chiTiet.SoLuong ?? 0) + 1;
            }
            else
            {
                GioHangChiTiet newChiTiet = new GioHangChiTiet
                {
                    MaChiTiet = Guid.NewGuid().ToString("N").Substring(0, 20),
                    MaGioHang = gioHang.MaGioHang,
                    MaSanPham = id,
                    SoLuong = 1,
                    Gia = sanPham.Gia
                };

                db.GioHangChiTiets.Add(newChiTiet);
            }

            gioHang.NgayCapNhat = DateTime.Now;

            db.SaveChanges();

            TempData["Success"] = "Sản phẩm đã được thêm vào giỏ hàng";

            return Redirect(Request.UrlReferrer.ToString());
        }

        // Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public ActionResult Xoa(string id)
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            var gioHang = db.GioHangs.FirstOrDefault(x => x.MaNguoiDung == user.MaNguoiDung);

            if (gioHang == null)
                return RedirectToAction("DanhSach");

            var chiTiet = db.GioHangChiTiets.FirstOrDefault(x => x.MaChiTiet == id && x.MaGioHang == gioHang.MaGioHang);

            if (chiTiet != null)
            {
                db.GioHangChiTiets.Remove(chiTiet);
                db.SaveChanges();
            }

            return RedirectToAction("DanhSach");
        }

        // Cập nhật số lượng
        [HttpPost]
        public ActionResult CapNhat(string maChiTiet, int? soLuong)
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            if (string.IsNullOrEmpty(maChiTiet))
                return RedirectToAction("DanhSach");

            if (soLuong == null || soLuong < 1)
                soLuong = 1;

            var gioHang = db.GioHangs.FirstOrDefault(x => x.MaNguoiDung == user.MaNguoiDung);

            if (gioHang == null)
                return RedirectToAction("DanhSach");

            var chiTiet = db.GioHangChiTiets.Include("SanPham").FirstOrDefault(x => x.MaChiTiet == maChiTiet && x.MaGioHang == gioHang.MaGioHang);

            if (chiTiet == null)
                return RedirectToAction("DanhSach");

            int tonKho = chiTiet.SanPham.SoLuong ?? 0;

            if (tonKho <= 0)
            {
                TempData["Error"] = "Sản phẩm " + chiTiet.SanPham.TenSanPham + " đã hết hàng";
                return RedirectToAction("DanhSach", "GioHang");
            }

            if (soLuong > tonKho)
            {
                TempData["Error"] = "Sản phẩm " + chiTiet.SanPham.TenSanPham +
                                    " số lượng chỉ còn " + tonKho + " sản phẩm trong kho";

                return RedirectToAction("DanhSach");
            }

            chiTiet.SoLuong = soLuong.Value;

            db.SaveChanges();

            return RedirectToAction("DanhSach");
        }
    }
}