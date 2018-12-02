using System;
using Hangfire;

namespace Batches
{
    public class ContainerJobActivator : JobActivator
    {
        private IServiceProvider _provider;

        public ContainerJobActivator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override object ActivateJob(Type jobType)
        {
            return _provider.GetService(jobType);
        }
    }
}
