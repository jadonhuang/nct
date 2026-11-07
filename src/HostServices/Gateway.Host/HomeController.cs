using Microsoft.AspNetCore.Mvc;

namespace Gateway.Host
{
    public class HomeController:Controller
    {
        public IActionResult Index()
        {

            return Content("index");
        }
    }
}
