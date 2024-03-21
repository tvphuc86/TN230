using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace thietbiphatsang.Controllers
{
    public class BaseController : Controller
        {
        doantn230Entities db = new doantn230Entities();

        // GET: Base
            
            public BaseController()
            {
           

            if (System.Web.HttpContext.Current.Session["QuyenSD"].ToString()!="Admin")
                {
                    System.Web.HttpContext.Current.Response.Redirect("/home/login");
                }

            }
            public ActionResult Logout()
            {
                Session["Username"] = "";
                Session["daidien"] = "";
                Session["QuyenSD"] = "";
                Session["ID"] = "";
                return Redirect("/Home");
            }
        }
    
}