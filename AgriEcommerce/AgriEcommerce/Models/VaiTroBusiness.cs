using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriEcommerce.Models
{
    public class VaiTroBusiness
    {
        public List<VaiTro> LayDanhSach()
        {
            return DataProvider.Entities.VaiTroes.ToList();
        }

        public VaiTro LayChiTiet(string maVaiTro)
        {
            VaiTro objVaiTro = DataProvider.Entities.VaiTroes.Where(x => x.MaVaiTro == maVaiTro).FirstOrDefault();
            return objVaiTro;
        }
    }
}