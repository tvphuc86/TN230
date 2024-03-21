using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Newtonsoft.Json;
using thietbiphatsang.Models;
using thietbiphatsang.Controllers;

namespace thietbiphatsang.Areas.Quantri.Controllers
{
    public class QuanTriController : BaseController
    {
        doantn230Entities db = new doantn230Entities();
        // GET: Quantri/QuanTri
        public ActionResult Trangchu()
        {
            int intTongdon = db.tb_DonDatHang.ToList().Count();
            ViewBag.TongDon = intTongdon;
            int intKhachHang = db.tb_TaiKhoan.ToList().Count();
            ViewBag.KhachHang = intKhachHang;
            var objLoiNhan = db.tb_DonDatHang.ToList();
            double LoiNhuan = 0;
            ViewBag.Sanpham = db.tb_ThietBi.Count();
            foreach(var item in objLoiNhan)
            {
                LoiNhuan = LoiNhuan + item.floatTongTien.Value;
            }
            ViewBag.DoanhThu = LoiNhuan;
            return View();
        }
        public ActionResult loaisanpham (int? page)
        {
            if (page == null) page = 1;
            var obj = db.tbLoaiThietBi.ToList().OrderBy(m=>m.strMaLoai);
            int pagesize = 2;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber,pagesize));
        }
        [HttpPost]
        public ActionResult themloaisanpham(FormCollection form)
        {
            var maloai = "";
            var ma = db.tbLoaiThietBi.ToList().LastOrDefault();
            if (ma == null)
            {
                maloai = "L0";
            }
            else maloai = "L" + (int.Parse(ma.strMaLoai.Substring(1)) +1 );
            var ten = form[1];
            var gioithieu = form[2];  
            var obj = new tbLoaiThietBi();
            obj.strMaLoai = maloai;
            obj.strTen = ten;
            obj.strGioiThieu = gioithieu;
            db.tbLoaiThietBi.Add(obj);
            db.SaveChanges();
            return RedirectToAction("loaisanpham");
        }
        public ActionResult chinhsualoai (string id)
        {
            
            return View(db.tbLoaiThietBi.Where(m=>m.strMaLoai.Equals(id)).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult chinhsualoai(string id , string ten, string gioithieu)
        {


                    var objLoai = db.tbLoaiThietBi.Where(m => m.strMaLoai.Equals(id)).FirstOrDefault();
                    objLoai.strGioiThieu = gioithieu;
                    objLoai.strTen = ten;
                    db.Entry(objLoai).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    
                    return Json("Lưu thành công",JsonRequestBehavior.AllowGet);
               

        }
        public ActionResult xoaloai(string id)
        {
            if (db.tb_ThietBi.Where(m => m.strMaLoai.Equals(id)).FirstOrDefault()!=null)
            {
                return Json("Không thể xóa do còn thiết bị loại này đang kinh doanh",JsonRequestBehavior.AllowGet);

            }
            else
            {
                db.tbLoaiThietBi.Remove(db.tbLoaiThietBi.Where(m => m.strMaLoai.Equals(id)).FirstOrDefault());
                db.SaveChanges();
            }
            return Json("Xóa thành công",JsonRequestBehavior.AllowGet);
        }
        public ActionResult sanpham(int? page)
        {

            var obj1 = new sanpham();
            if (page == null) page = 1;
            var obj = db.tb_ThietBi.ToList().OrderBy(m=>m.strMaLoai);
            int pagesize = 10;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber, pagesize));
        }
        [HttpPost]
        public ActionResult themsp (FormCollection form, HttpPostedFileBase File)
        {
            var masp = "";
            if (db.tb_ThietBi.ToList().LastOrDefault() == null)
            {
                masp = "SP0";
            }
            else masp = "SP" + (int.Parse(db.tb_ThietBi.ToList().LastOrDefault().strMaThietBi.ToString().Substring(2))+1);
            var obj = new tb_ThietBi();
           
            obj.strMaThietBi = masp;
            obj.strTenThietBi = form[0];
            obj.strMaNSX = form[1];
            obj.strMaLoai = form[2];
            obj.floatGia = float.Parse( form[3]);
            obj.intSoLuongTon = int.Parse( form[4]);
            db.tb_ThietBi.Add(obj);
            db.SaveChanges();
            if (File.ContentLength > 0)
            {
                var anh = new tb_AnhThietBi();
                string filename = masp + File.FileName;
                anh.strMaThietBi = masp;
                anh.strAnh = filename;
                db.tb_AnhThietBi.Add(anh);
                db.SaveChanges();
                string folder = Path.Combine(Server.MapPath("~/Areas/Quantri/Content/img/sanpham/"), filename);
                File.SaveAs(folder);
            }
           
            return RedirectToAction("sanpham");
        }
        public ActionResult suasp( string id, string ten,string mansx, string maloai, int soluong,float gia)
        {
            var obj = db.tb_ThietBi.Where(m => m.strMaThietBi.Equals(id)).FirstOrDefault();
            obj.strTenThietBi = ten;
            obj.strMaNSX = mansx;
            obj.strMaLoai = maloai;
            obj.intSoLuongTon = soluong;
            obj.floatGia = gia;
            try
            {
                db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("Lưu thành công",JsonRequestBehavior.AllowGet);
            }
           catch(Exception)
            {
                return Json("Có lỗi khi lưu", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult xoasp(string id)
        {
            var obj = db.tb_ThietBi.Where(m => m.strMaThietBi.Equals(id)).FirstOrDefault();
            foreach (var item in obj.tb_AnhThietBi.ToList())
            {
                string path = Server.MapPath("~/Areas/Quantri/Content/img/sanpham/" + item.strAnh);
                System.IO.File.Delete(path);
            }
            foreach (var item in obj.tb_AnhThietBi.ToList())
            {
                db.tb_AnhThietBi.Remove(item);
                db.SaveChanges();
            } 
            foreach (var item in obj.tb_BaiViet.ToList())
            {
                db.tb_BaiViet.Remove(item);
                db.SaveChanges();
            }    
            db.tb_ThietBi.Remove(obj);
            db.SaveChanges();
            
            return Json("Xóa thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult nsx(int? page)
        {

            var obj1 = new sanpham();
            if (page == null) page = 1;
            var obj = db.tb_NSX.ToList().OrderBy(m => m.str_Ma);
            int pagesize = 10;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber, pagesize));
        }
        public ActionResult themnsx (string ten)
        {
            var ma = "";
            if (db.tb_NSX.ToList().LastOrDefault() != null)
                ma = "NSX" + (int.Parse(db.tb_NSX.ToList().LastOrDefault().str_Ma.Substring(3)) + 1);
            else ma = "NSX0";

            var obj = new tb_NSX();
            obj.str_Ma = ma;
            obj.strTen = ten;
            db.tb_NSX.Add(obj);
            db.SaveChanges();
            return Json("Thêm thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult suansx(string id, string ten)
        {
            var obj = db.tb_NSX.Where(m => m.str_Ma.Equals(id)).FirstOrDefault();
            obj.strTen = ten;
            db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json("Lưu thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult xoansx(string id) {
            var dk = db.tb_ThietBi.Where(m => m.strMaNSX.Equals(id)).FirstOrDefault();
            if (dk != null) {
                return Json("Không thể xóa nhà sản xuất có sản phẩm đang kinh doanh", JsonRequestBehavior.AllowGet);

            }
            var obj = db.tb_NSX.Where(m => m.str_Ma.Equals(id)).FirstOrDefault();
            db.tb_NSX.Remove(obj);
            db.SaveChanges();
            return Json("Xóa thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult khuyenmai(int? page)
        {

            var obj1 = new sanpham();
            if (page == null) page = 1;
            var obj = db.tb_KhuyenMai.ToList().OrderBy(m => m.dateBatDau);
            int pagesize = 10;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber, pagesize));
        }
        public ActionResult themkm(string ten ,DateTime bd, DateTime kt)
        {
            if (bd.Date <= DateTime.Today.Date || kt.Date < bd.Date) return Json("Ngày không hợp lệ. Ngày kết thúc phải lớn hơn ngày bắt đầu, ngày bắt đầu phải lớn hơn ngày hiện tại",JsonRequestBehavior.AllowGet);
            var ma = "";
            if (db.tb_KhuyenMai.ToList().LastOrDefault() == null)
                ma = "KM01";
            else ma = "KM" + (int.Parse(db.tb_KhuyenMai.ToList().LastOrDefault().strMaDotKM.ToString().Substring(2)) + 1);
            var km = new tb_KhuyenMai();
            km.strMaDotKM = ma;
            km.ten = ten;
            km.dateBatDau = bd;
            km.dateKetThuc = kt;
            db.tb_KhuyenMai.Add(km);
            db.SaveChanges();
            return Json("Thêm thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult suakm (string id, string ten,DateTime bd, DateTime kt)
        {
            if (bd.Date <= DateTime.Today.Date || kt.Date < bd.Date) return Json("Ngày không hợp lệ. Ngày kết thúc phải lớn hơn ngày bắt đầu, ngày bắt đầu phải lớn hơn ngày hiện tại", JsonRequestBehavior.AllowGet);
            var obj = db.tb_KhuyenMai.Where(m => m.strMaDotKM.Equals(id)).FirstOrDefault();
            obj.ten = ten;
            obj.dateBatDau = bd;
            obj.dateKetThuc = kt;
            db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json("Lưu thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult xoakm(string id)
        {
            var dk = db.tb_ChiTietKM.Where(m => m.strMaDotKM.Equals(id)).FirstOrDefault();
            if (dk != null)
            {
                return Json("Không thể xóa đợt khuyến mãi có sản phẩm đang hưởng khuyến mãi", JsonRequestBehavior.AllowGet);
            }
            var obj = db.tb_KhuyenMai.Where(m => m.strMaDotKM.Equals(id)).FirstOrDefault();
            db.tb_KhuyenMai.Remove(obj);
            db.SaveChanges();
            return Json("Xóa thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult spkm(int? page)
        {

            var obj1 = new sanpham();
            if (page == null) page = 1;
            var obj = db.tb_ChiTietKM.ToList().OrderBy(m => m.id);
            int pagesize = 10;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber, pagesize));
        }
        public ActionResult loc(string nsx, string loaisp)
        {
            
            if (loaisp.Equals("0"))
            {
                var obj = from item in db.tb_ThietBi where item.strMaNSX.Equals(nsx) select new { masp = item.strMaThietBi, tensp = item.strTenThietBi };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }

           if (nsx.Equals("0"))
            {
                var obj = from item in db.tb_ThietBi where item.strMaLoai.Equals(loaisp) select new { masp = item.strMaThietBi, tensp = item.strTenThietBi };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var obj = from item in db.tb_ThietBi where item.strMaLoai.Equals(loaisp) && item.strMaNSX.Equals(nsx) select new { masp = item.strMaThietBi, tensp = item.strTenThietBi };
                return Json(JsonConvert.SerializeObject(obj), JsonRequestBehavior.AllowGet);
            }
          
        }
        public ActionResult themspkm (string makm , string sp, float tile)
        {
            
            var sp1 = sp.Split(',');
            foreach (var item in sp1)
            {
                var obj = new tb_ChiTietKM();
                obj.strMaDotKM = makm;
                obj.strMaThietBi = item;
                obj.floatTiLeGiam = tile;
                db.tb_ChiTietKM.Add(obj);
                db.SaveChanges();
            }    
            return Json("Thêm thành công",JsonRequestBehavior.AllowGet);
        }
        public ActionResult suaspkm(string makm, string masp,float tile)
        {
            var obj = db.tb_ChiTietKM.Where(m => m.strMaDotKM.Equals(makm) && m.strMaThietBi.Equals(masp)).FirstOrDefault();
            obj.floatTiLeGiam = tile;
            db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json("Lưu thành công", JsonRequestBehavior.AllowGet);
        }
        public ActionResult xoaspkm (string makm,string masp)
        {
            var obj = db.tb_ChiTietKM.Where(m => m.strMaDotKM.Equals(makm) && m.strMaThietBi.Equals(masp)).FirstOrDefault();
            db.tb_ChiTietKM.Remove(obj);
            db.SaveChanges();
            return Json("Xóa thành công", JsonRequestBehavior.AllowGet);

        }
        public ActionResult thembaiviet (string matb, string tieude, string noidung)
        {
            if (tieude.Equals("") || noidung.Equals(""))
            {
                return Json("Vui lòng điền đủ các trường", JsonRequestBehavior.AllowGet);
            }
            var mabv = "BV"+matb+db.tb_BaiViet.Where(m=>m.strMaThietBi.Equals(matb)).Count();
            var obj = new tb_BaiViet();
            obj.strMaThietBi = matb;
            obj.strMaBaiViet = mabv;
            obj.strTieuDe = tieude;
            obj.strNoiDung = noidung;
            obj.dateTaoLuc = DateTime.Now.Date;
            db.tb_BaiViet.Add(obj);
            db.SaveChanges();
            return Json("Thêm thành công", JsonRequestBehavior.AllowGet);
        }
    }
}