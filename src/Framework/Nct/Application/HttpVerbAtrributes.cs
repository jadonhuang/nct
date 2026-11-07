using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nct.Application
{


    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : Attribute
    {

    }

}