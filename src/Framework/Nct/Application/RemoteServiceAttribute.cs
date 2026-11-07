using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nct.Application
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class RemoteServiceAttribute:Attribute
    {

        public bool IsEnabled { get; set; } = true;

        public string Name { get; set; }

        public RemoteServiceAttribute()
        {
        }

        public RemoteServiceAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public RemoteServiceAttribute(string name)
        {
            Name = name;
        }

    }
}