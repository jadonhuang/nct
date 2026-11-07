using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nct.Authorization;
using Nct.Module.Core;
using Nct.Module.Form.Annexs.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Nct.Module.Form.Annexs
{


    public class AnnexAppService : CrudAppService<Annex, AnnexDto, string, QueryDto, CreateAnnexDto, CreateAnnexDto>, IAnnexAppService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnnexAppService(IRepository<Annex, string> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
        {

            _httpContextAccessor = httpContextAccessor;
        }



        private Annex UpLoadFile(IFormFile file, bool isLocal = false)
        {

            var fileMeta = FileStoreHelper.SaveFile(file, isLocal: isLocal);

            var model = new Annex
            {
                Id = Guid.NewGuid().ToString(),
                Name = fileMeta.Name,
                ContentType = fileMeta.ContentType,
                Size = fileMeta.Size,
                Path = fileMeta.Path,
                Extension = Path.GetExtension(fileMeta.Path)
            };

            return model;



        }


        [DisableRequestSizeLimit]
        public async Task<List<Annex>> UploadFiles(string fileId, bool isLocal = false)
        {

            var models = new List<Annex>();

            if (fileId.IsNotEmpty())
            {
                var model = await EditFile(fileId);
                models.Add(model);

            }
            else
            {

                foreach (var file in _httpContextAccessor.HttpContext.Request.Form.Files)
                {

                    var model = UpLoadFile(file, isLocal);
                    models.Add(model);
                    await Repository.InsertAsync(model);


                }
            }
            return models;


        }


        private async Task<Annex> EditFile(string fileId)
        {


            var files = _httpContextAccessor.HttpContext.Request.Form.Files;
            if (!files.Any())
                throw new BusinessException("无上传文件");

            var file = files[0];

            var annex = await Repository.GetAsync(fileId) ?? throw new BusinessException("无附件信息");

            var model = UpLoadFile(file);
            model.Id = fileId;
            model.CreateTime = annex.CreateTime;
            await Repository.UpdateAsync(model);
            return model;


        }


        [Post]
        public async Task<List<Annex>> GetListByIds(string[] ids)
        {
            if (ids == null) throw new BusinessException("ids is null");
            return await Repository.GetAllListAsync(m => ids.Contains(m.Id));

        }



        [RemoteService(false)]
        public override Task<AnnexDto> Create(CreateAnnexDto dto)
        {
            return base.Create(dto);
        }

        [Anonymous, Get]
        [HttpGet("/api/annex/download/{fileId}")]
        public IActionResult Download(string fileId)
        {

            var annex = Repository.Get(fileId);
            if (annex == null) return new NotFoundResult();

            var stream = FileStoreHelper.GetFileStream(annex.Path);
            if (stream == null)
                return new NotFoundResult();

            return new FileStreamResult(stream, "application/octet-stream") { FileDownloadName = annex.Name };



        }

    }
}
