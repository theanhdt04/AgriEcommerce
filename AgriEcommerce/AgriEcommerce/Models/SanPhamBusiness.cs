using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriEcommerce.Models
{
    public class SanPhamBusiness
    {
        public List<SanPham> TimKiemSanPham(string tuKhoa, string maDM)
        {
            IQueryable<SanPham> lstSanPham = DataProvider.Entities.SanPhams.AsNoTracking();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                lstSanPham = lstSanPham.Where(x => x.MaSanPham.Contains(tuKhoa) || x.TenSanPham.Contains(tuKhoa) || x.MoTa.Contains(tuKhoa));
            }

            if (!string.IsNullOrEmpty(maDM))
            {
                lstSanPham = lstSanPham.Where(x => x.MaDanhMuc == maDM);
            }

            return lstSanPham.ToList();
        }

        public List<SanPham> LayDanhSach()
        {
            return DataProvider.Entities.SanPhams.AsNoTracking().ToList();
        }

        public SanPham LayChiTiet(string maSanPham)
        {
            SanPham objSanPham = DataProvider.Entities.SanPhams.AsNoTracking().FirstOrDefault(x => x.MaSanPham == maSanPham);
            return objSanPham;
        }

        public bool ThemMoi(SanPham objSanPham)
        {
            if (objSanPham == null)
                return false;

            // kiểm tra mã sản phẩm đã tồn tại chưa
            var sanPham = DataProvider.Entities.SanPhams.FirstOrDefault(x => x.MaSanPham == objSanPham.MaSanPham);

            if (sanPham != null)
            {
                return false; // mã đã tồn tại
            }

            DataProvider.Entities.SanPhams.Add(objSanPham);
            DataProvider.Entities.SaveChanges();

            return true;
        }

        public bool CapNhat(SanPham objSanPham, string maSanPham)
        {
            SanPham objSanPhamDb = DataProvider.Entities.SanPhams.Where(x => x.MaSanPham == maSanPham).FirstOrDefault();
            if (objSanPhamDb == null) return false;
            objSanPham.NgayTao = objSanPhamDb.NgayTao;
            DataProvider.Entities.Entry(objSanPhamDb).CurrentValues.SetValues(objSanPham);
            return DataProvider.Entities.SaveChanges() > 0 ? true : false;
        }

        public bool Xoa(string maSanPham)
        {
            var objSanPham = DataProvider.Entities.SanPhams.FirstOrDefault(x => x.MaSanPham == maSanPham);

            if (objSanPham != null)
            {
                DataProvider.Entities.SanPhams.Remove(objSanPham);
                DataProvider.Entities.SaveChanges();
                return true;
            }

            return false;
        }
    }
}