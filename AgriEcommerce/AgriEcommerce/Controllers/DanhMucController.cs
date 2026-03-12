using AgriEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class DanhMucController : AdminBaseController
    {
        DanhMucBusiness danhMucBusiness = new DanhMucBusiness();

        public ActionResult danhsach()
        {
            List<DanhMuc> lstDanhMuc = danhMucBusiness.LayDanhSach();
            return View(lstDanhMuc);
        }

        public ActionResult ThemMoi()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMoi(DanhMuc objDanhMuc)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã danh mục đã tồn tại chưa
                var check = danhMucBusiness.LayChiTiet(objDanhMuc.MaDanhMuc);

                if (check != null)
                {
                    ModelState.AddModelError("MaDanhMuc", "Mã danh mục đã tồn tại!");
                    return View(objDanhMuc);
                }

                objDanhMuc.NgayTao = DateTime.Now;
                objDanhMuc.NgayCapNhat = DateTime.Now;

                bool ketQua = danhMucBusiness.ThemMoi(objDanhMuc);

                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }

            return View(objDanhMuc);
        }

        public ActionResult SuaThongTin(string maDanhMuc)
        {
            DanhMuc objDanhMuc = danhMucBusiness.LayChiTiet(maDanhMuc);
            if (objDanhMuc != null)
            {
                return View(objDanhMuc);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTin(DanhMuc objDanhMuc, string maDanhMuc)
        {
            if (ModelState.IsValid)
            {
                objDanhMuc.NgayCapNhat = DateTime.Now;
                bool ketQua = danhMucBusiness.CapNhat(objDanhMuc, maDanhMuc);
                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }
            return View(objDanhMuc);
        }

        public ActionResult XoaThongTin(string maDanhMuc)
        {
            if (!string.IsNullOrEmpty(maDanhMuc))
            {
                bool ketQua = danhMucBusiness.Xoa(maDanhMuc);
                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }
            return View();
        }
    }

}
