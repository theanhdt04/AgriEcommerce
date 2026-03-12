using AgriEcommerce.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class SanPhamController : AdminBaseController
    {
        SanPhamBusiness sanPhamBusiness = new SanPhamBusiness();

        public ActionResult danhsach(string tuKhoa, string maDanhMuc, int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            HienThiDanhMuc(maDanhMuc);

            List<SanPham> lstSanPham = sanPhamBusiness.TimKiemSanPham(tuKhoa, maDanhMuc);

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.MaDanhMuc = maDanhMuc;

            return View(lstSanPham.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ThemMoi()
        {
            HienThiDanhMuc();
            HienThiDonViTinh();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMoi(SanPham objSanPham, HttpPostedFileBase fUpload)
        {
            if (fUpload == null || fUpload.ContentLength == 0)
            {
                ModelState.AddModelError("HinhAnh", "Vui lòng chọn hình ảnh sản phẩm!");
            }

            else if (!fUpload.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("HinhAnh", "File phải là hình ảnh!");
            }

            if (ModelState.IsValid)
            {
                objSanPham.NgayTao = DateTime.Now;
                objSanPham.NgayCapNhat = DateTime.Now;

                if (fUpload != null && fUpload.ContentLength > 0)
                {
                    string path = Server.MapPath("~/Content/images/");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = Path.GetFileName(fUpload.FileName);

                    string fullPath = Path.Combine(path, fileName);

                    fUpload.SaveAs(fullPath);

                    objSanPham.HinhAnh = fileName;
                }

                bool ketQua = sanPhamBusiness.ThemMoi(objSanPham);

                if (ketQua)
                {
                    return RedirectToAction("DanhSach");
                }
                else
                {
                    ModelState.AddModelError("MaSanPham", "Mã sản phẩm đã tồn tại!");
                }
            }

            HienThiDanhMuc(objSanPham.MaDanhMuc);
            HienThiDonViTinh(objSanPham.MaDonViTinh);

            return View(objSanPham);
        }

        public ActionResult SuaThongTin(string maSanPham)
        {
            SanPham objSanPham = sanPhamBusiness.LayChiTiet(maSanPham);

            if (objSanPham != null)
            {
                HienThiDanhMuc(objSanPham.MaDanhMuc);
                HienThiDonViTinh(objSanPham.MaDonViTinh);
                return View(objSanPham);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTin(SanPham objSanPham,string maSanPham, HttpPostedFileBase fUpload)
        {
            if (ModelState.IsValid)
            {
                objSanPham.NgayCapNhat = DateTime.Now;

                string anhSanPham = "";
                if (fUpload != null && fUpload.ContentLength > 0)
                {

                    fUpload.SaveAs(Server.MapPath("~/Content/images/" + fUpload.FileName));
                    anhSanPham = fUpload.FileName;
                }
                if (!string.IsNullOrEmpty(anhSanPham))
                {
                    objSanPham.HinhAnh = anhSanPham;
                }
 
                bool ketQua = sanPhamBusiness.CapNhat(objSanPham, maSanPham);

                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }

            HienThiDanhMuc(objSanPham.MaDanhMuc);
            HienThiDonViTinh(objSanPham.MaDonViTinh);
            return View(objSanPham);
        }

        public ActionResult XoaThongTin(string maSanPham)
        {
            if (!string.IsNullOrEmpty(maSanPham))
            {
                bool ketQua = sanPhamBusiness.Xoa(maSanPham);

                if (ketQua)
                {
                    return RedirectToAction("DanhSach");
                }        
            }

            return RedirectToAction("DanhSach");
        }

        private void HienThiDanhMuc(string maDM = null)
        {
            DanhMucBusiness danhMucBusiness = new DanhMucBusiness();
            List<DanhMuc> lstDanhMuc = danhMucBusiness.LayDanhSach();
            ViewBag.DanhMuc = new SelectList(lstDanhMuc, "MaDanhMuc", "TenDanhMuc", maDM);
        }

        private void HienThiDonViTinh(string maDVT = null)
        {
            DonViTinhBusiness donViTinhBusiness = new DonViTinhBusiness();
            List<DonViTinh> lstDonViTinh = donViTinhBusiness.LayDanhSach();
            ViewBag.DonViTinh = new SelectList(lstDonViTinh, "MaDonViTinh", "TenDonViTinh", maDVT);
        }

    }
}