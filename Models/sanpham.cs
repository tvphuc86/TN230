using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace thietbiphatsang.Models
{
    public class sanpham
    {
        public List<tb_NSX> lstNSX { get; set; }
        public List<tb_AnhThietBi> anh { get; set; }
        public List<tbLoaiThietBi> loai{ get; set; }

    }
}