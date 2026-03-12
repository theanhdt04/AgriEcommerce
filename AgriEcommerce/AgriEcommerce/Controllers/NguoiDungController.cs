using AgriEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgriEcommerce.Controllers
{
    public class NguoiDungController : AdminBaseController
    {
        NguoiDungBusiness nguoiDungBusiness = new NguoiDungBusiness();
        // GET: NguoiDung
        public ActionResult danhsach()
        {
            List<NguoiDung> lstNguoiDung = nguoiDungBusiness.LayDanhSach();
            return View(lstNguoiDung);
        }

        public ActionResult ThemMoi()
        {
            HienThiVaiTro();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMoi(NguoiDung objNguoiDung)
        {
            if (ModelState.IsValid)
            {
                // Chuẩn hóa dữ liệu
                objNguoiDung.MaNguoiDung = objNguoiDung.MaNguoiDung.Trim().ToUpper();
                objNguoiDung.Email = objNguoiDung.Email.Trim();

                // Kiểm tra trùng mã người dùng
                var checkMa = nguoiDungBusiness.LayChiTiet(objNguoiDung.MaNguoiDung);
                if (checkMa != null)
                {
                    ModelState.AddModelError("MaNguoiDung", "Mã người dùng đã tồn tại!");
                    HienThiVaiTro(objNguoiDung.MaVaiTro);
                    return View(objNguoiDung);
                }

                // Kiểm tra trùng email
                var checkEmail = nguoiDungBusiness.LayTheoEmail(objNguoiDung.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng!");
                    HienThiVaiTro(objNguoiDung.MaVaiTro);
                    return View(objNguoiDung);
                }

                var checkPhone = nguoiDungBusiness.LayTheoDienThoai(objNguoiDung.DienThoai);
                if (checkPhone != null)
                {
                    ModelState.AddModelError("DienThoai", "Số điện thoại đã được sử dụng!");
                    HienThiVaiTro(objNguoiDung.MaVaiTro);
                    return View(objNguoiDung);
                }

                objNguoiDung.NgayTao = DateTime.Now;
                objNguoiDung.NgayCapNhat = DateTime.Now;

                // Mã hóa mật khẩu
                objNguoiDung.MatKhau = Common.ToMD5(objNguoiDung.MatKhau);

                bool ketQua = nguoiDungBusiness.ThemMoi(objNguoiDung);

                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }

            HienThiVaiTro(objNguoiDung.MaVaiTro);
            return View(objNguoiDung);
        }

        public ActionResult SuaThongTin(string maNguoiDung)
        {
            NguoiDung objNguoiDung = nguoiDungBusiness.LayChiTiet(maNguoiDung);
            if (objNguoiDung != null)
            {
                HienThiVaiTro(objNguoiDung.MaVaiTro);
                return View(objNguoiDung);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaThongTin(NguoiDung objNguoiDung)
        {
            if (ModelState.IsValid)
            {
                if (objNguoiDung != null)
                {
                    objNguoiDung.NgayCapNhat = DateTime.Now;

                    string matKhauMaHoa = Common.ToMD5(objNguoiDung.MatKhau);
                    if (objNguoiDung.MatKhau != matKhauMaHoa)
                    {
                        objNguoiDung.MatKhau = matKhauMaHoa;
                    }

                    bool ketQua = nguoiDungBusiness.CapNhat(objNguoiDung);
                    if (ketQua)
                    {
                        return RedirectToAction("danhsach");
                    }
                }
            }
            HienThiVaiTro(objNguoiDung.MaVaiTro);
            return View(objNguoiDung);
        }

        public ActionResult XoaThongTin(string maNguoiDung)
        {
            if (!string.IsNullOrEmpty(maNguoiDung))
            {
                bool ketQua = nguoiDungBusiness.Xoa(maNguoiDung);
                if (ketQua)
                {
                    return RedirectToAction("danhsach");
                }
            }
            return View();
        }

        private void HienThiVaiTro(string maVaiTro = null)
        {
            VaiTroBusiness vaiTroBusiness = new VaiTroBusiness();
            List<VaiTro> lstVaiTro = vaiTroBusiness.LayDanhSach();
            ViewBag.VaiTro = new SelectList(lstVaiTro, "MaVaiTro", "TenVaiTro", maVaiTro);
        }
    }
}