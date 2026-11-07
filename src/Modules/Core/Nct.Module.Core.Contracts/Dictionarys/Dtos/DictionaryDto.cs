using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;
using Nct.Models;
using System.Linq;
using System.Text.Json.Serialization;

namespace Nct.Module.Core.Dictionarys.Dtos
{

    [Map(typeof(Dictionary))]
    public class DictionaryDto : IEntityDto, ITreeNode<int, DictionaryDto>
    {

        public int Id { get; set; }


        public int ParentId { get; set; }


        public string Code { get; set; }


        public string Name { get; set; }


        public string Descpt { get; set; }

        [JsonIgnore]
        public string Path { get; set; }


        public int Sort { get; set; }


        public int Depth { get; set; }


        public IList<DictionaryDto> Children { get; set; }


      

    }
}
