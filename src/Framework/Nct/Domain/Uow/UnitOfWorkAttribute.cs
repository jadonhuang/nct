using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Domain.Uow
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UnitOfWorkAttribute : Attribute
    {
        public TimeSpan? Timeout { get; set; }


        public bool? IsTransactional { get; set; }

        public bool Enable { get; set; } = true;

        public UnitOfWorkAttribute()
        {

        }

        public UnitOfWorkAttribute(bool enable, bool isTransactional = true)
        {
            Enable = enable;
            IsTransactional = isTransactional;
        }

        public UnitOfWorkAttribute(int timeOut)
        {
            Timeout = TimeSpan.FromSeconds(timeOut);
        }

        public UnitOfWorkOptions CreateOptions()
        {
            return new UnitOfWorkOptions
            {
                IsolationLevel = System.Data.IsolationLevel.ReadCommitted,
                Timeout = Timeout,
                IsTransactional = IsTransactional
            };

        }

    }
}
