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
    
    public partial class tb_HoSoCaNhan
    {
        public string strTen { get; set; }
        public string strDiaChi { get; set; }
        public Nullable<System.DateTime> dateNgaySinh { get; set; }
        public string strID { get; set; }
    
        public virtual tb_TaiKhoan tb_TaiKhoan { get; set; }
    }
}
