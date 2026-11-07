using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Configuration
{
    public class DbConnectionOptions
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings : Dictionary<string, string>
    {
        public const string DefaultName = "Default";

        public string Default
        {
            get => this.GetOrDefault(DefaultName);
            set => this[DefaultName] = value;
        }
    }

}
