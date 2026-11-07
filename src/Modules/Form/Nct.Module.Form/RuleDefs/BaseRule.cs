using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.RuleDefs
{
    public abstract class BaseRule
    {

        public virtual Task BeforeSave(RuleContext context)
        { 
          
           return Task.CompletedTask;
        }

        public virtual Task AfterSave(RuleContext context)
        {

            return Task.CompletedTask;

        }


        public virtual Task BeforeDelete(RuleContext context)
        {

            return Task.CompletedTask;

        }

        public virtual Task AfterDelete(RuleContext context)
        {

            return Task.CompletedTask;

        }


        public virtual Task AfterGetListData(RuleContext context)
        {

            return Task.CompletedTask;

        }


    }
}
