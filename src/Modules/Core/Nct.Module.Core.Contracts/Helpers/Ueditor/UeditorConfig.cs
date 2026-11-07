using Nct.Json;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Text.Json;
using Nct.EmbeddedResources;

namespace Nct.Module.Core.Ueditor
{

    public static class UeditorConfig
    {

        private static JsonElement? _element;

        private static dynamic GetItems()
        {
            IEmbeddedResourceManager resourceManager =IocManager.Instance.Resolve<IEmbeddedResourceManager>();
            var resource = resourceManager.GetResource("/ueditor.json");
            StreamReader reader = new StreamReader(resource.GetStream());
            string json = reader.ReadToEnd();
            return json.ToObject<JsonElement>();
        }

        public static JsonElement Element
        {
            get
            {
                if (_element == null)
                {
                    _element = GetItems();
                }
                return _element.Value;
            }
        }




        public static string[] GetStringList(string key)
        {

            return Element.GetProperty(key).EnumerateArray().Select(m => m.GetString()).ToArray();

        }

        public static string GetString(string key)
        {
            return Element.GetProperty(key).GetString();
        }

        public static int GetInt(string key)
        {
            return Element.GetProperty(key).GetInt32();
        }
    }
}
