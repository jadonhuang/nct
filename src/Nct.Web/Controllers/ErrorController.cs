using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nct.AspNetCore;
using Nct.Authorization;
using Nct.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nct.Web.Controllers
{
    [Anonymous]
    public class ErrorController : BaseController
    {


        public ILogger Logger { get; set; }

        public ErrorController()
        {
            Logger = NullLogger.Instance;

        }

        public IActionResult ErrorWithCode()
        {
            return View();
        }

        [HttpGet("/ErrorWithCode/{statusCode}")]
        public IActionResult ErrorWithCode(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("404");
            }

            return View("500");
        }

        [HttpGet("/Error")]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            if (error != null)
            {
                Logger.Error(error);
            }

            return View("500");
        }


    }
}
