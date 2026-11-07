using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Json
{
    public class JsonException:Exception
    {
        public JsonException(string message) : base(message) { }
    }
}
