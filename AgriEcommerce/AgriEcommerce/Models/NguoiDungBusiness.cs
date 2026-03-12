using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriEcommerce.Models
{
    public class NguoiDungBusiness
    {
        public List<NguoiDung> LayDanhSach()
        {
            return DataProvider.Entities.NguoiDungs.ToList();
        }

        public NguoiDung LayChiTiet(string maNguoiDung)
        {
            NguoiDung objNguoiDung = DataProvider.Entities.NguoiDungs.Find(maNguoiDung);
            return objNguoiDung;
        }

        public NguoiDung KiemTraDangNhap(string email)
        {
            NguoiDung objNguoiDung = DataProvider.Entities.NguoiDungs.Where(e => e.Email.ToLower().Equals(email.ToLower())).FirstOrDefault();
            return objNguoiDung;
        }

        public bool ThemMoi(NguoiDung objNguoiDung)
        {
            if (objNguoiDung != null)
            {
                DataProvider.Entities.NguoiDungs.Add(objNguoiDung);
                DataProvider.Entities.SaveChanges();
                return true;
            }
            return false;
        }

        public NguoiDung LayTheoEmail(string email)
        {
            return DataProvider.Entities.NguoiDungs.FirstOrDefault(x => x.Email == email);
        }

        public NguoiDung LayTheoDienThoai(string dienThoai)
        {
            return DataProvider.Entities.NguoiDungs.FirstOrDefault(x => x.DienThoai == dienThoai);
        }

        public bool CapNhat(NguoiDung objNguoiDung)
        {
            NguoiDung objNguoiDungDb = LayChiTiet(objNguoiDung.MaNguoiDung);
            if (objNguoiDungDb == null) return false;
            objNguoiDung.NgayTao = objNguoiDungDb.NgayTao;
            DataProvider.Entities.Entry(objNguoiDungDb).CurrentValues.SetValues(objNguoiDung);
            DataProvider.Entities.SaveChanges();
            return true;
        }

        public bool Xoa(string maNguoiDung)
        {
            NguoiDung objNguoiDung = LayChiTiet(maNguoiDung);

            if (objNguoiDung != null)
            {
                // 1. Lấy danh sách đơn hàng
                var donHang = DataProvider.Entities.DonHangs.Where(x => x.MaNguoiDung == maNguoiDung).ToList();

                // 2. Xóa chi tiết đơn hàng
                foreach (var dh in donHang)
                {
                    var chiTiet = DataProvider.Entities.DonHangChiTiets.Where(x => x.MaDonHang == dh.MaDonHang).ToList();

                    DataProvider.Entities.DonHangChiTiets.RemoveRange(chiTiet);
                }

                // 3. Xóa đơn hàng
                DataProvider.Entities.DonHangs.RemoveRange(donHang);

                // 4. Xóa giỏ hàng
                var gioHang = DataProvider.Entities.GioHangs.Where(x => x.MaNguoiDung == maNguoiDung).ToList();

                DataProvider.Entities.GioHangs.RemoveRange(gioHang);

                // 5. Xóa người dùng
                DataProvider.Entities.NguoiDungs.Remove(objNguoiDung);

                DataProvider.Entities.SaveChanges();

                return true;
            }

            return false;
        }

    }
}