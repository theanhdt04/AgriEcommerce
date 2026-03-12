using AgriEcommerce.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class TrangChuController : Controller
    {
        // GET: TrangChu
        public ActionResult DanhSach( string ma, int? page)
        {
            SanPhamBusiness sanPhamBusiness = new SanPhamBusiness();

            int pageSize = 8; // số sản phẩm mỗi trang
            int pageNumber = (page ?? 1);

            List<SanPham> lstSanPham = sanPhamBusiness.TimKiemSanPham("", ma);

            ViewBag.Ma = ma;

            return View(lstSanPham.ToPagedList(pageNumber, pageSize));
        }

        [ChildActionOnly]
        public ActionResult DanhMucTrangChu()
        {
            DanhMucBusiness danhMucBusiness = new DanhMucBusiness();
            List<DanhMuc> lstDanhMuc = danhMucBusiness.LayDanhSach();
            return PartialView("_NavAgriEcommerce", lstDanhMuc);
        }

        public ActionResult LienHe()
        {
            return View();
        }

    }
}