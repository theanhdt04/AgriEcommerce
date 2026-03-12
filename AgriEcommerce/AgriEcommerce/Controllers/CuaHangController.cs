using AgriEcommerce.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class CuaHangController : Controller
    {
        SanPhamBusiness sanPhamBusiness = new SanPhamBusiness();

        public ActionResult DanhSach(string tuKhoa, string ma, int? page)
        {
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            List<SanPham> lstSanPham = sanPhamBusiness.TimKiemSanPham(tuKhoa, ma);

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.Ma = ma;

            return View(lstSanPham.ToPagedList(pageNumber, pageSize));
        }

        [ChildActionOnly]
        public ActionResult DanhMucTrangChu()
        {
            DanhMucBusiness danhMucBusiness = new DanhMucBusiness();
            List<DanhMuc> lstDanhMuc = danhMucBusiness.LayDanhSach();
            return PartialView("_NavCuaHang", lstDanhMuc);
        }

        public ActionResult ChiTietSanPham(string ma)
        {
            SanPham objSanPham = sanPhamBusiness.LayChiTiet(ma);
            if(objSanPham != null)
            {
                return View(objSanPham);
            }
            return HttpNotFound();
        }
    }
}