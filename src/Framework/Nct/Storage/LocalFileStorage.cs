using Nct.Extentions;
using Nct.Helpers;
using Nct.Ioc;
using System.Collections.Generic;
using System.IO;

namespace Nct.Storage
{
    public class LocalFileStorage : IFileStorage
    {


        private static string GetPhysicalPath(string path)
        {
            return PathHelper.WebPath(path);
        }

        public void CopyFile(string sourceFileName, string destFileName, bool overwrite = false)
        {
            sourceFileName = GetPhysicalPath(sourceFileName);
            destFileName = GetPhysicalPath(destFileName);
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public void CreateDirectory(string path)
        {
            path = GetPhysicalPath(path);
            Directory.CreateDirectory(path);
        }

        private static void CheckFileExist(string path, FileCreateType fileCreateType)
        {
            if (File.Exists(path) && fileCreateType == FileCreateType.CreateNew)
                throw new IOException($"{path} exists");

        }

        public void CreateFile(string path, Stream stream, FileCreateType fileCreateType = FileCreateType.CreateNew)
        {
            path = GetPhysicalPath(path);

            CheckFileExist(path, fileCreateType);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            byte[] buffer = new byte[512];

            using (var fs = File.Create(path))
            {
                while (true)
                {

                    var len = stream.Read(buffer, 0, buffer.Length);
                    if (len == 0) break;

                    fs.Write(buffer, 0, len);

                }

            }

        }

        public void CreateFile(string path, string content, FileCreateType fileCreateType = FileCreateType.CreateNew)
        {

            path = GetPhysicalPath(path);
            CheckFileExist(path, fileCreateType);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var sw = File.CreateText(path))
            {
                sw.Write(content);

            }

        }

        public void DeleteDirectory(string path, bool recursive = false)
        {
            path = GetPhysicalPath(path);
            Directory.Delete(path, recursive);
        }

        public void DeleteFile(string path)
        {
            path = GetPhysicalPath(path);
            File.Delete(path);
        }

        public void DeleteFiles(List<string> pathes)
        {
            foreach (var path in pathes)
            {
                DeleteFile(path);
            }
        }

        public bool ExistDirectory(string path)
        {
            path = GetPhysicalPath(path);
            return Directory.Exists(path);
        }

        public bool ExistFile(string path)
        {
            path = GetPhysicalPath(path);
            return File.Exists(path);
        }

        public byte[] GetFileContent(string path)
        {
            path = GetPhysicalPath(path);
            if (File.Exists(path))
                return File.ReadAllBytes(path);

            return null;
        }

        public List<string> GetFileEntries(string dirName)
        {
            dirName = GetPhysicalPath(dirName);
            return [.. Directory.GetFileSystemEntries(dirName)];
        }

        public List<string> GetFiles(string dirName)
        {
            dirName = GetPhysicalPath(dirName);
            return [.. Directory.GetFiles(dirName, "*")];
        }

        public void MoveFile(string sourceFileName, string destFileName, bool overwrite = false)
        {
            sourceFileName = GetPhysicalPath(sourceFileName);
            destFileName = GetPhysicalPath(destFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
            File.Move(sourceFileName, destFileName, overwrite);
        }

        public string GetUrl(string path)
        {
            return path.EnsureStartWith('/');
        }

        public Stream GetFileStream(string path)
        {

            path = GetPhysicalPath(path);
            if (File.Exists(path))
                return File.OpenRead(path);

            return null;
        }
    }
}
