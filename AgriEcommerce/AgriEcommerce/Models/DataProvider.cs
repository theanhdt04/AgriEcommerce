using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgriEcommerce.Models
{
    public class DataProvider
    {
        static AgriEcommerceEntities _Entities = new AgriEcommerceEntities();
        public static AgriEcommerceEntities Entities
        {
            get { return _Entities; }
        }
    }
}