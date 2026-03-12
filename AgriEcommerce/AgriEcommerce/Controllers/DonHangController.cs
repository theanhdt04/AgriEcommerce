using AgriEcommerce.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class DonHangController : Controller
    {
        private AgriEcommerceEntities db = new AgriEcommerceEntities();

        public ActionResult ThanhToan()
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            var gioHang = db.GioHangs.FirstOrDefault(x => x.MaNguoiDung == user.MaNguoiDung);

            if (gioHang == null)
                return RedirectToAction("DanhSach", "GioHang");

            var chiTiet = db.GioHangChiTiets.Include(x => x.SanPham).Where(x => x.MaGioHang == gioHang.MaGioHang).ToList();

            if (!chiTiet.Any())
                return RedirectToAction("DanhSach", "GioHang");

            //Kiểm tra tồn kho trước khi hiển thị trang thanh toán
            foreach (var item in chiTiet)
            {
                int tonKho = item.SanPham.SoLuong ?? 0;
                int soLuongGio = item.SoLuong ?? 0;

                if (tonKho <= 0)
                {
                    TempData["Error"] = "Sản phẩm " + item.SanPham.TenSanPham + " đã hết hàng";
                    return RedirectToAction("DanhSach", "GioHang");
                }

                if (soLuongGio > tonKho)
                {
                    TempData["Error"] = "Sản phẩm " + item.SanPham.TenSanPham +
                                        " chỉ còn " + tonKho + " sản phẩm trong kho";

                    return RedirectToAction("DanhSach", "GioHang");
                }
            }

            return View(chiTiet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatHang(string hoTen, string dienThoai, string diaChi, string ghiChu)
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            var gioHang = db.GioHangs.FirstOrDefault(x => x.MaNguoiDung == user.MaNguoiDung);

            if (gioHang == null)
                return RedirectToAction("DanhSach", "GioHang");

            var chiTietGio = db.GioHangChiTiets.Include(x => x.SanPham).Where(x => x.MaGioHang == gioHang.MaGioHang).ToList();

            if (!chiTietGio.Any())
                return RedirectToAction("DanhSach", "GioHang");

            //Kiểm tra tồn kho trước khi tạo đơn hàng
            foreach (var item in chiTietGio)
            {
                var sanPham = db.SanPhams.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);

                if (sanPham == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại";
                    return RedirectToAction("DanhSach", "GioHang");
                }

                int tonKho = sanPham.SoLuong ?? 0;
                int soLuongGio = item.SoLuong ?? 0;

                if (tonKho <= 0)
                {
                    TempData["Error"] = "Sản phẩm " + sanPham.TenSanPham + " đã hết hàng";
                    return RedirectToAction("DanhSach", "GioHang");
                }

                if (soLuongGio > tonKho)
                {
                    TempData["Error"] = "Sản phẩm " + sanPham.TenSanPham +
                                        " số lượng chỉ còn " + tonKho + " sản phẩm trong kho";

                    return RedirectToAction("DanhSach", "GioHang");
                }
            }


            //Tính tổng tiền
            decimal tongTien = chiTietGio.Sum(x => (x.SanPham.Gia ?? 0) * (x.SoLuong ?? 0));


            //Tao đơn hàng
            DonHang donHang = new DonHang
            {
                MaDonHang = Guid.NewGuid().ToString("N").Substring(0, 20),
                MaNguoiDung = user.MaNguoiDung,
                TongTien = tongTien,
                HoTenNguoiNhan = hoTen,
                DienThoai = dienThoai,
                DiaChiGiaoHang = diaChi,
                GhiChu = ghiChu,
                TrangThai = "ChoXuLy",
                DaDuyet = false,
                NgayDat = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            db.DonHangs.Add(donHang);
            db.SaveChanges();


            //Tạo chi tiết đơn hàng + trừ kho
            foreach (var item in chiTietGio)
            {
                var sanPham = db.SanPhams.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);

                if (sanPham != null)
                {
                    sanPham.SoLuong = (sanPham.SoLuong ?? 0) - (item.SoLuong ?? 0);

                    if (sanPham.SoLuong <= 0)
                    {
                        sanPham.SoLuong = 0;
                        sanPham.ConHang = false;
                    }

                    db.Entry(sanPham).State = EntityState.Modified;
                }

                DonHangChiTiet ct = new DonHangChiTiet
                {
                    MaChiTiet = Guid.NewGuid().ToString("N").Substring(0, 20),
                    MaDonHang = donHang.MaDonHang,
                    MaSanPham = item.MaSanPham,
                    SoLuong = item.SoLuong,
                    DonGia = item.SanPham.Gia
                };

                db.DonHangChiTiets.Add(ct);
            }


            //Xoa giỏ hàng
            db.GioHangChiTiets.RemoveRange(chiTietGio);
            db.SaveChanges();

            return RedirectToAction("DatHangThanhCong");
        }


        public ActionResult DatHangThanhCong()
        {
            return View();
        }


        public ActionResult LichSuDonHang()
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            var donHangs = db.DonHangs.Where(x => x.MaNguoiDung == user.MaNguoiDung).OrderByDescending(x => x.NgayDat).ToList();

            return View(donHangs);
        }


        public ActionResult ChiTietDonHang(string id)
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            var donHang = db.DonHangs
                .FirstOrDefault(x => x.MaDonHang == id && x.MaNguoiDung == user.MaNguoiDung);

            if (donHang == null)
                return RedirectToAction("LichSuDonHang");

            var chiTiet = db.DonHangChiTiets.Include(x => x.SanPham).Where(x => x.MaDonHang == id).ToList();

            ViewBag.DonHang = donHang;

            return View(chiTiet);
        }


        public ActionResult HuyDonHang(string id)
        {
            var user = Session["UserOnline"] as NguoiDung;

            if (user == null)
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = Request.RawUrl });

            var donHang = db.DonHangs
                .FirstOrDefault(x => x.MaDonHang == id && x.MaNguoiDung == user.MaNguoiDung);

            if (donHang == null)
                return RedirectToAction("LichSuDonHang");

            if (donHang.TrangThai == "ChoXuLy")
            {
                var chiTiet = db.DonHangChiTiets.Where(x => x.MaDonHang == donHang.MaDonHang).ToList();

                foreach (var item in chiTiet)
                {
                    var sanPham = db.SanPhams.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);

                    if (sanPham != null)
                    {
                        sanPham.SoLuong += item.SoLuong ?? 0;
                        sanPham.ConHang = true;
                    }
                }

                donHang.TrangThai = "Huy";
                donHang.NgayCapNhat = DateTime.Now;

                db.SaveChanges();
            }

            return RedirectToAction("LichSuDonHang");
        }
    }
}