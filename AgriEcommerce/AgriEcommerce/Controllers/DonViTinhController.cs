using AgriEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class DonViTinhController : AdminBaseController
    {
        DonViTinhBusiness donViTinhBusiness = new DonViTinhBusiness();

        public ActionResult danhsach()
        {
            List<DonViTinh> lstDonViTinh = donViTinhBusiness.LayDanhSach();
            return View(lstDonViTinh);
        }

        public ActionResult ThemMoi()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMoi(DonViTinh objDonViTinh)
        {
            if (ModelState.IsValid)
            {
                var check = donViTinhBusiness.LayChiTiet(objDonViTinh.MaDonViTinh);

                if (check != null)
                {
                    ModelState.AddModelError("MaDonViTinh", "Mã đơn vị tính đã tồn tại!");
                    return View(objDonViTinh);
                }

                bool ketQua = donViTinhBusiness.ThemMoi(objDonViTinh);

                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }

            return View(objDonViTinh);
        }

        public ActionResult SuaThongTin(string maDonViTinh)
        {
            DonViTinh objDonViTinh = donViTinhBusiness.LayChiTiet(maDonViTinh);

            if (objDonViTinh != null)
            {
                return View(objDonViTinh);
            }

            return RedirectToAction("danhsach");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTin(DonViTinh objDonViTinh)
        {
            if (ModelState.IsValid)
            {
                bool ketQua = donViTinhBusiness.CapNhat(objDonViTinh);

                if (ketQua)
                {
                    return RedirectToAction("DanhSach");
                }
            }

            return View(objDonViTinh);
        }

        public ActionResult XoaThongTin(string maDonViTinh)
        {
            if (!string.IsNullOrEmpty(maDonViTinh))
            {
                bool ketQua = donViTinhBusiness.Xoa(maDonViTinh);

                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }

            return RedirectToAction("danhsach");
        }
    }
}