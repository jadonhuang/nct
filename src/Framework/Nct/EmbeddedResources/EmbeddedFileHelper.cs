using System;
using System.Text;
using System.IO;
using System.Web;

namespace Nct.EmbeddedResources
{
    public static class EmbeddedResourceHelper
    {


        public static string EncodeAsResourcesSubPath(string subPath)
        {

            var path = subPath.Replace("/", ".").TrimStart('.');
            if (!path.EndsWith("."))
            {
                path += ".";
            }

            return path;

        }


        public static string EncodeAsResourcesPath(string subPath)
        {

            var builder = new StringBuilder(subPath.Length);
            subPath = subPath.TrimStart('~');
            var indexOfLastSeperator = subPath.LastIndexOf('/');
            if (indexOfLastSeperator != -1)
            {

                for (int i = 0; i <= indexOfLastSeperator; i++)
                {
                    var currentChar = subPath[i];

                    if (currentChar == '/')
                    {
                        if (i != 0)
                        {
                            builder.Append('.');
                        }
                        continue;
                    }

                    if (currentChar == '-')
                    {
                        builder.Append('_');
                        continue;
                    }

                    if (currentChar == '.')
                    {
                        var nextChar = subPath[i + 1];
                        if (char.IsDigit(nextChar))
                        {
                            builder.Append("._");
                        }
                        else
                        {
                            builder.Append(".");
                        }
                        continue;
                    }

                    builder.Append(currentChar);
                }
            }

            if (subPath.Length > indexOfLastSeperator + 1)
            {
                builder.Append(subPath.Substring(indexOfLastSeperator + 1));
            }

            return builder.ToString();
        }





    }
}