using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.MessageQueue
{

    [AttributeUsage(AttributeTargets.Class)]
    public class MessageStoreTypeAttribute :Attribute
    {

        public MessageStoreTypeAttribute(Type type)
        {     
            Type = type;
        }

        public Type Type { get; set; }

        public MessageStoreTypeAttribute(string typeName)
        {

            Type = Type.GetType(typeName);

        }


    }
}
