using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Http.ApiDescription
{
    public class ParameterApiDescription
    {

        public string Name { get; }

        public bool IsSimpleType { get; }

        public ParameterApiDescription(string name, bool isSimpleType)
        {
            Name = name;
            IsSimpleType = isSimpleType;
        }
    }
}
