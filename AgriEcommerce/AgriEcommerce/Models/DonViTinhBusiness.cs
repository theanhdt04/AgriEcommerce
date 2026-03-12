using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriEcommerce.Models
{
    public class DonViTinhBusiness
    {
        public List<DonViTinh> LayDanhSach()
        {
            return DataProvider.Entities.DonViTinhs.ToList();
        }
        public DonViTinh LayChiTiet(string maDonViTinh)
        {
            DonViTinh objDonViTinh = DataProvider.Entities.DonViTinhs.Where(x => x.MaDonViTinh == maDonViTinh).FirstOrDefault();
            return objDonViTinh;
        }

        public bool ThemMoi(DonViTinh objDonViTinh)
        {
            if (objDonViTinh != null)
            {
                DataProvider.Entities.DonViTinhs.Add(objDonViTinh);
                DataProvider.Entities.SaveChanges();
                return true;
            }
            return false;
        }

        public bool CapNhat(DonViTinh objDonViTinh)
        {
            DonViTinh obj = DataProvider.Entities.DonViTinhs.FirstOrDefault(x => x.MaDonViTinh == objDonViTinh.MaDonViTinh);

            if (obj == null) return false;

            obj.TenDonViTinh = objDonViTinh.TenDonViTinh;
            obj.MoTa = objDonViTinh.MoTa;

            DataProvider.Entities.SaveChanges();

            return true;
        }

        public bool Xoa(string maDonViTinh)
        {
            DonViTinh objDonViTinh = LayChiTiet(maDonViTinh);

            if (objDonViTinh != null)
            {
                DataProvider.Entities.DonViTinhs.Remove(objDonViTinh);
                DataProvider.Entities.SaveChanges();
                return true;
            }
            return false;
        }
    }
}