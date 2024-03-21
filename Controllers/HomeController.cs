using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using thietbiphatsang.Models;
namespace thietbiphatsang.Controllers
{
    public class HomeController : Controller
    {
        doantn230Entities db = new doantn230Entities();
        public ActionResult index(int? page)
        {
            if (page == null) page = 1;
            var obj = db.tb_ThietBi.ToList().OrderBy(m => m.strMaThietBi);
            int pagesize = 6;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber, pagesize));
        }
        public ActionResult loctheoloai(int? page, string maloai)
        {
            if (page == null) page = 1;
            var obj = db.tb_ThietBi.Where(m=>m.strMaLoai.Equals(maloai)).ToList().OrderBy(m => m.strMaThietBi);
            int pagesize = 6;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber, pagesize));
        }
        public ActionResult loctheonsx(int? page, string nsx)
        {
            if (page == null) page = 1;
            var obj = db.tb_ThietBi.Where(m => m.strMaNSX.Equals(nsx)).ToList().OrderBy(m => m.strMaThietBi);
            int pagesize = 6;
            int pagenumber = (page ?? 1);
            return View(obj.ToPagedList(pagenumber, pagesize));
        }
        public ActionResult loctheokm(int? page, string makm)
        {
            if (page == null) page = 1;
            var obj = db.tb_ChiTietKM.Where(m => m.strMaDotKM.Equals(makm)).ToList();
            var obj1 = new List<tb_ThietBi>();
            foreach (var item in obj)
            {
                var obj2 = db.tb_ThietBi.Where(m => m.strMaThietBi.Equals(item.strMaThietBi)).FirstOrDefault();
                obj1.Add(obj2);
            }
            int pagesize = 6;
            int pagenumber = (page ?? 1);
            return View(obj1.ToPagedList(pagenumber, pagesize));
        }
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(string id,string matkhau)
        {
            var obj = db.tb_TaiKhoan.Where(m => m.strID.Equals(id) && matkhau.Equals(m.strPassword)).FirstOrDefault();
            if (obj!=null)
            {
                if (obj.boolQuyenSD == true)
                    Session["QuyenSD"] = "Admin";
                else Session["QuyenSD"] = "User";
                Session["ID"] = obj.strID;
                Session["Username"] = obj.tb_HoSoCaNhan.strTen;
                return Json("Bạn đăng nhập bằng tài khoản"+"\""+Session["QuyenSD"].ToString()+"\""+"thành công", JsonRequestBehavior.AllowGet);
            }
            else return Json("Sai id hoặc mật khẩu", JsonRequestBehavior.AllowGet);
        }
        public ActionResult dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult dangky (string id, string matkhau,string matkhau1,string ten,DateTime ngaysinh,string diachi)
        {
            if (id.Equals("")|| matkhau.Equals("") || matkhau1.Equals("") || ten.Equals("") || ngaysinh.Equals("") || diachi.Equals("") )
            {
                return Json("Vui lòng nhập đủ các trường", JsonRequestBehavior.AllowGet);
            }
            if (!matkhau.Equals(matkhau1))
            {
                return Json("Xác nhận mật khẩu sai", JsonRequestBehavior.AllowGet);
            }
            if (DateTime.Today.Year - ngaysinh.Date.Year < 16)
            {
                return Json("Ngày sinh không hợp lệ, phải lớn hơn 16 tuổi", JsonRequestBehavior.AllowGet);

            }
            var obj = db.tb_TaiKhoan.Where(m => m.strID.Equals(id)).FirstOrDefault();
            if (obj != null)
            {
                return Json("Id đã tồn tại vui lòng chọn id khác", JsonRequestBehavior.AllowGet);

            }

            var obj1 = new tb_HoSoCaNhan();
            var obj2 = new tb_TaiKhoan();
            obj1.strID = id;
            obj1.dateNgaySinh = ngaysinh;
            obj1.strTen = ten;
            obj1.strDiaChi = diachi;
            db.tb_HoSoCaNhan.Add(obj1);
            db.SaveChanges();
            obj2.strID = id;
            obj2.strPassword = matkhau;
            obj2.boolQuyenSD = false;
            db.tb_TaiKhoan.Add(obj2);
            db.SaveChanges();
            return Json("Đăng ký thành công, đang nhặp nào", JsonRequestBehavior.AllowGet);

        }
        public ActionResult thongtin(string masp)
        {
            return View(db.tb_BaiViet.Where(m=>m.strMaThietBi.Equals(masp)).ToList().OrderByDescending(n=>n.dateTaoLuc).FirstOrDefault());
        }
        public ActionResult ShoppingCart()
        {

            return View();
        }

        //Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public JsonResult AddToCart(string id)
        {
            List<CartItem> listCartItem;
            if (Session["ShoppingCart"] == null)
            {
                //Create New Shopping Cart Session 
                listCartItem = new List<CartItem>();
                listCartItem.Add(new CartItem { Quality = 1, productOrder = db.tb_ThietBi.Where(m => m.strMaThietBi == id).FirstOrDefault() });
                Session["ShoppingCart"] = listCartItem;
            }
            else
            {
                bool flag = false;
                listCartItem = (List<CartItem>)Session["ShoppingCart"];
                foreach (CartItem item in listCartItem)
                {
                    if (item.productOrder.strMaThietBi == id)
                    {
                        item.Quality++; flag = true;
                        break;
                    }
                }

                if (!flag)
                    listCartItem.Add(new CartItem { Quality = 1, productOrder = db.tb_ThietBi.Where(m => m.strMaThietBi == id).FirstOrDefault() });
                Session["ShoppingCart"] = listCartItem;
            }
            //Count item in shopping cart 
            int cartcount = 0;
            List<CartItem> ls = (List<CartItem>)Session["ShoppingCart"];
            foreach (CartItem item in ls)
            {
                cartcount += item.Quality;
            }
            return Json(new { ItemAmount = cartcount });
        }

    }
}