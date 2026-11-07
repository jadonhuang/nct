using Microsoft.AspNetCore.Mvc;
using Nct.Authorization;
using Nct.Http.ApiDescription;
using Nct.Json;

namespace Nct.AspNetCore.ApiDescription
{
    public class ApiDescriptionController : BaseController
    {
        private readonly IApiDescriptionModelProvider _descptProvider;
        public ApiDescriptionController(IApiDescriptionModelProvider descptProvider)
        {
            _descptProvider = descptProvider;


        }

        [Anonymous]
        public IActionResult Index()
        {
            var model = _descptProvider.Create();
            var str = model.ToJson();
            var m=str.ToObject<ApplicationApiDescription>();
            return Json(model);


        }

    }
}
