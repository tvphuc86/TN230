using System.Web.Mvc;

namespace thietbiphatsang.Areas.Quantri
{
    public class QuantriAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Quantri";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Quantri_default",
                "Quantri/{controller}/{action}/{id}",
                new { action = "trangchu", id = UrlParameter.Optional }
            );
        }
    }
}