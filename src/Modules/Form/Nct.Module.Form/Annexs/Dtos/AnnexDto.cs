using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Form.Annexs.Dtos
{

    [Map(typeof(Annex))]
    public class AnnexDto : IEntityDto<string>
    {

        public string Id { get; set; }


        public string Name { get; set; }


        public long Size { get; set; }


        public string ContentType { get; set; }


        public string Path { get; set; }


        public string Extension { get; set; }


    }
}
