//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace thietbiphatsang
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_BaiViet
    {
        public string strMaBaiViet { get; set; }
        public string strMaThietBi { get; set; }
        public string strTieuDe { get; set; }
        public string strNoiDung { get; set; }
        public Nullable<System.DateTime> dateTaoLuc { get; set; }
    
        public virtual tb_ThietBi tb_ThietBi { get; set; }
    }
}
