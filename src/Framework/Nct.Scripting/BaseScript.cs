using Nct.Ioc;
using Nct.Scripting;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Scripting
{
    public abstract class BaseScript
    {

        public virtual object Execute(dynamic context)
        {
            return null;
        }

        public virtual Task<object> ExecuteAsync(dynamic context)
        {
            return null;
        }


    }
}
