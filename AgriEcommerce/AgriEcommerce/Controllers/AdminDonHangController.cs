using AgriEcommerce.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class AdminDonHangController : AdminBaseController
    {
        private AgriEcommerceEntities db = new AgriEcommerceEntities();

        public ActionResult DanhSach(int? page)
        {
            int pageSize = 7;
            int pageNumber = (page ?? 1);

            var donHangs = db.DonHangs.OrderByDescending(x => x.NgayDat).ToPagedList(pageNumber, pageSize);

            return View(donHangs);
        }

        public ActionResult ChiTiet(string id)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDonHang == id);
            if (donHang == null)
                return RedirectToAction("danhsach");
            var chiTiet = db.DonHangChiTiets.Include(x => x.SanPham).Where(x => x.MaDonHang == id).ToList();
            ViewBag.DonHang = donHang;
            return View(chiTiet);
        }

        public ActionResult DuyetDon(string id)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDonHang == id);
            if (donHang == null)
                return RedirectToAction("danhsach");
            if (donHang.TrangThai == "ChoXuLy")
            {
                donHang.TrangThai = "DangGiao";
                donHang.DaDuyet = true;
                donHang.NgayCapNhat = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("danhsach");
        }

        public ActionResult HoanThanh(string id)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDonHang == id);
            if (donHang == null)
                return RedirectToAction("danhsach");
            donHang.TrangThai = "HoanThanh";
            donHang.NgayCapNhat = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("danhsach");
        }

        public ActionResult HuyDon(string id)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDonHang == id);
            if (donHang == null)
                return RedirectToAction("danhsach");
            donHang.TrangThai = "Huy";
            donHang.NgayCapNhat = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("danhsach");
        }

        public ActionResult XoaDonHang(string id)
        {
            DonHang dh = db.DonHangs.Find(id);
            if (dh != null)
            {
                db.DonHangs.Remove(dh);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSach");
        }
    }
}