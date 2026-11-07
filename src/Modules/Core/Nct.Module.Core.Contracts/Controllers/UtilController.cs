using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nct.AspNetCore;
using Nct.Authorization;
using Nct.Module.Core.Ueditor;
using System;
using System.IO;

namespace Nct.Module.Core.Controllers
{

    [Anonymous]
    public class UtilController : BaseController
    {

        private readonly IConfiguration _configuration;
     
        public UtilController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (culture != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(culture, culture)), cookieOptions);
            }

            return LocalRedirect(returnUrl);
        }



        [HttpPost]
        [DisableRequestSizeLimit]
        public RetResult Upload(bool isLocal=false)
        {

            var result = new RetResult();
            var file = Request.Form.Files[0];
            var fileInfo = FileStoreHelper.SaveFile(file,isLocal: isLocal);
            var fileName = $"/{fileInfo.Path}";
            return result.Success(fileName);

        }



        [DisableRequestSizeLimit]
        public ContentResult Ue()
        {
            string contentType = "text/html";
            string content;
            string action = Request.Query["action"];
            string callback = Request.Query["callback"];

            switch (action)
            {
                case "config":

                    content = UeditorConfig.Element.ToJson();
                    if (!callback.IsEmpty())
                    {
                        content = $"{callback}({content})";
                        contentType = "application/javascript";

                    };
                    break;

                case "uploadimage":
                    content = UEditorUpload(new UeditorUploadConfig()
                    {
                        AllowExtensions = UeditorConfig.GetStringList("imageAllowFiles"),
                        PathFormat = UeditorConfig.GetString("imagePathFormat"),
                        SizeLimit = UeditorConfig.GetInt("imageMaxSize"),
                        UploadFieldName = UeditorConfig.GetString("imageFieldName")
                    });
                    break;

                case "uploadvideo":
                    content = UEditorUpload(new UeditorUploadConfig()
                    {
                        AllowExtensions = UeditorConfig.GetStringList("videoAllowFiles"),
                        PathFormat = UeditorConfig.GetString("videoPathFormat"),
                        SizeLimit = UeditorConfig.GetInt("videoMaxSize"),
                        UploadFieldName = UeditorConfig.GetString("videoFieldName")
                    });
                    break;
                case "uploadfile":

                    content = UEditorUpload(new UeditorUploadConfig()
                    {
                        AllowExtensions = UeditorConfig.GetStringList("fileAllowFiles"),
                        PathFormat = UeditorConfig.GetString("filePathFormat"),
                        SizeLimit = UeditorConfig.GetInt("fileMaxSize"),
                        UploadFieldName = UeditorConfig.GetString("fileFieldName")
                    });
                    break;

                default:
                    content = new
                    {
                        state = "action 参数为空或者 action 不被支持。"
                    }.ToJson();
                    break;



            }

            return Content(content, contentType);



        }


        private static string GetStateMessage(UeditorUploadState state)
        {
            return state switch
            {
                UeditorUploadState.Success => "SUCCESS",
                UeditorUploadState.FileAccessError => "文件访问出错，请检查写入权限",
                UeditorUploadState.SizeLimitExceed => "文件大小超出服务器限制",
                UeditorUploadState.TypeNotAllow => "不允许的文件格式",
                UeditorUploadState.NetworkError => "网络错误",
                _ => "未知错误",
            };
        }


        private string UEditorUpload(UeditorUploadConfig uploadConfig)
        {

            UeditorUploadResult result = new() { State = UeditorUploadState.Success };

            string uploadFileName;

            Stream stream = null;

            if (uploadConfig.Base64)
            {
                uploadFileName = uploadConfig.Base64Filename;
                var uploadFileBytes = Convert.FromBase64String(Request.Form[uploadConfig.UploadFieldName]);
                stream = new MemoryStream(uploadFileBytes);
            }
            else
            {
                var file = Request.Form.Files[uploadConfig.UploadFieldName];

                uploadFileName = file.FileName;
                var ext = Path.GetExtension(uploadFileName);

                if (!uploadConfig.AllowExtensions.Any(m => m.IgnoreCaseEqual(ext)))
                {
                    result.State = UeditorUploadState.TypeNotAllow;
                }
                else if (file.Length > uploadConfig.SizeLimit)
                {
                    result.State = UeditorUploadState.SizeLimitExceed;
                }
                else
                {
                    stream = file.OpenReadStream();
                }
            }

            if (result.State == UeditorUploadState.Success)
            {
                var savePath = UeditorPathFormatter.Format(uploadFileName, uploadConfig.PathFormat);
                try
                {

                    FileStoreHelper.SaveFile(savePath, stream);
                    result.Url = savePath;
                    result.State = UeditorUploadState.Success;
                }
                catch (Exception e)
                {
                    result.State = UeditorUploadState.FileAccessError;
                    result.ErrorMessage = e.Message;
                }


            }

            var resultStr = new
            {
                state = GetStateMessage(result.State),
                url = result.Url,
                title = uploadFileName,
                original = uploadFileName,
                error = result.ErrorMessage
            }.ToJson();

            return resultStr;

        }

        [Route("api/util/GetCheckCodeImg")]
        public RetResult GetCheckCodeImg()
        {

            var ret = new RetResult();
            var useMathCal = _configuration["SysCheckCodeType"] != "1";
            var data = CheckCodeHelper.Create(useMathCal);
            return ret.Success(new { data.Id, Img = CommonHelper.BytesToBase64Img(data.Img) });

        }


        [HttpGet]
        public IActionResult HeartBeat()
        {
            return Content("ok");
        }

    }
}
