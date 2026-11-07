using Nct.Ioc;
using System.Collections.Generic;
using System.IO;

namespace Nct.Storage
{
    public interface IFileStorage: ISingletonDependency
    {

        byte[] GetFileContent(string path);


        Stream GetFileStream(string path);


        void CreateFile(string path, Stream stream, FileCreateType fileCreateType = FileCreateType.CreateNew);

     
        void CreateFile(string path, string content, FileCreateType fileCreateType = FileCreateType.CreateNew);


        void CreateDirectory(string path);

     
        bool ExistFile(string path);


        bool ExistDirectory(string path);


        void DeleteDirectory(string path, bool recursive = false);


        void DeleteFile(string path);


        void DeleteFiles(List<string> pathes);

   
        void CopyFile(string sourceFileName, string destFileName, bool overwrite = false);

     
        void MoveFile(string sourceFileName, string destFileName, bool overwrite = false);

     
        List<string> GetFileEntries(string dirName);


        List<string> GetFiles(string dirName);


        string GetUrl(string path);

     
    }
}
