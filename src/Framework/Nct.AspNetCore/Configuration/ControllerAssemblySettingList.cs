using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nct.AspNetCore.Configuration
{
   public class ControllerAssemblySettingList:List<ControllerAssemblySetting>
    {
        public ControllerAssemblySetting GetOrNull(Type controllerType)
        {
            return this.FirstOrDefault(controllerSetting => controllerSetting.Assembly == controllerType.Assembly);
        }

    }
}
