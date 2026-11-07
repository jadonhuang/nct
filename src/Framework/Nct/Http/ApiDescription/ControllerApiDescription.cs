using Nct.Extentions;
using System.Collections.Generic;
using System.Data.Common;


namespace Nct.Http.ApiDescription
{
    public class ControllerApiDescription
    {
        public Dictionary<string, ActionApiDescription> Actions { get; set; }

        public string Name { get; }

        public string[] Interfaces { get; }

        public ControllerApiDescription(string name, string[] interfaces)
        {
            Name = name;
            Actions = new Dictionary<string, ActionApiDescription>();
            Interfaces = interfaces;
        }

        public ActionApiDescription AddAction(ActionApiDescription action)
        {

            return Actions[action.Name] = action;
        }




    }
}
