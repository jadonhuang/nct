using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nct.AspNetCore;
using Nct.Authorization;

namespace Nct.Web.Controllers
{



    [Anonymous]
    public class HomeController(IConfiguration configuration) : BaseController
    {


        private readonly IConfiguration _configuration = configuration;
      

        public IActionResult Index()
        {


            //var adminEntry = _configuration.GetAdminEntry();
            //if (adminEntry.IsNotEmpty())
            //{
            //    return Redirect("/" + adminEntry);

            //}


            return Content("Index");

        }




    }
}
