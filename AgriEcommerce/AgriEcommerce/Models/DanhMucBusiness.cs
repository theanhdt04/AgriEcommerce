using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriEcommerce.Models
{
    public class DanhMucBusiness
    {
        public List<DanhMuc> LayDanhSach()
        {
            return DataProvider.Entities.DanhMucs.ToList();
        }

        public DanhMuc LayChiTiet(string maDanhMuc)
        {
            DanhMuc objDanhMuc = DataProvider.Entities.DanhMucs.Where(x => x.MaDanhMuc == maDanhMuc).FirstOrDefault();
            return objDanhMuc;
        }

        public bool ThemMoi(DanhMuc objDanhMuc)
        {
            if (objDanhMuc != null)
            {
                DataProvider.Entities.DanhMucs.Add(objDanhMuc);
                DataProvider.Entities.SaveChanges();
                return true;
            }
            return false;
        }
        public bool CapNhat(DanhMuc objDanhMuc, string maDanhMuc)
        {
            DanhMuc objDanhMucDb = DataProvider.Entities.DanhMucs.Where(x => x.MaDanhMuc == maDanhMuc).FirstOrDefault();
            if (objDanhMucDb == null) return false;
            objDanhMuc.NgayTao = objDanhMucDb.NgayTao;
            DataProvider.Entities.Entry(objDanhMucDb).CurrentValues.SetValues(objDanhMuc);
            return DataProvider.Entities.SaveChanges() > 0 ? true : false;
        }

        public bool Xoa(string maDanhMuc)
        {
            DanhMuc objDanhMuc = LayChiTiet(maDanhMuc);
            if (objDanhMuc != null)
            {
                DataProvider.Entities.DanhMucs.Remove(objDanhMuc);
                DataProvider.Entities.SaveChanges();
                return true;
            }
            return false;
        }
    }
}