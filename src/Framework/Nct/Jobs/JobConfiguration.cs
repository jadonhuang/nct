using Nct.Ioc;
using Nct.Jobs.Qz;
using System;
using System.Collections.Generic;

namespace Nct.Jobs
{
    public class JobConfiguration
    {

        public bool Enabled { get; set; } = true;

        public void UseQuartz(IReadOnlyList<Type> types)
        {

            if (Enabled)
            {

                IocManager.Instance.RegisterIfNot<IJobWorker, QuartzJobWorker>();
                JobWorkerBuilder.Create().Register(types).Build().Start();


            }



        }

    }
}
