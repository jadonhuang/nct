using Microsoft.AspNetCore.Mvc;
using Nct.Authorization;
using Nct.Http.ScriptProxy;

namespace Nct.AspNetCore.ScriptProxy
{
    public class ApiScriptProxyController : BaseController
    {
        private readonly IApiScriptBuilder _apiScriptBuilder;
        public ApiScriptProxyController(IApiScriptBuilder apiScriptBuilder)
        {
            _apiScriptBuilder = apiScriptBuilder;

        }

        [Anonymous]
        public IActionResult Index(ScriptProxyRequestDto dto)
        {
            string script = _apiScriptBuilder.Build(dto.Module,dto.AddModulePrefix);

#if !DEBUG
            script = NUglify.Uglify.Js(script).Code;
#endif
            return Content(script, "application/javascript");


        }
    }
}
