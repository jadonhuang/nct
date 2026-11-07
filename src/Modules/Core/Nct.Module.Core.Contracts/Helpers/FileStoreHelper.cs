using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Nct.Storage;
using System;
using System.IO;

namespace Nct.Module.Core
{
    public static class FileStoreHelper
    {

   
        public static string CheckFile(IFormFile file, string saveName = null)
        {

            var configuration = IocManager.Instance.Resolve<IConfiguration>();
            var allowExts = configuration["Store:AllowExts"].Split(',');
            var maxSize = long.Parse(configuration["Store:MaxSize"]);

            var fileName = file.FileName;
            var ext = Path.GetExtension(fileName);

            if (!allowExts.Any(e => e.IgnoreCaseEqual(ext))) throw new BusinessException($" {fileName}不允许的文件类型");

            if (file.Length > maxSize) throw new BusinessException($"{fileName}文件过大");

            saveName ??= $"{DateTime.Now:yyMMdd}/{DateTime.Now:HHmmssff}{CommonHelper.GetRandom(6, true)}{ext}";

            var filePath = $"upload/{saveName}";

            return filePath;


        }

        private static IFileStorage GetFileStorage(bool isLocal)
        {
            if (isLocal)
                return IocManager.Instance.Resolve<LocalFileStorage>();
            return IocManager.Instance.Resolve<IFileStorage>();
        }

        public static FileMetaInfo SaveFile(IFormFile file, string saveName = null, bool isLocal = false)
        {

            var filePath = CheckFile(file, saveName);
            var stream = file.OpenReadStream();
            GetFileStorage(isLocal).CreateFile(filePath, stream);

            var fileMeta = new FileMetaInfo
            {
                Path = filePath,
                ContentType = file.ContentType,
                Size = file.Length,
                Name = file.FileName
            };

            return fileMeta;
        }

        public static void SaveFile(string path, Stream stream, bool isLocal = false)
        {

            GetFileStorage(isLocal).CreateFile(path, stream);
        }


        public static Stream GetFileStream(string path, bool isLocal = false)
        {
            return GetFileStorage(isLocal).GetFileStream(path);
        }

    }
}
