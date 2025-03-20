using Quartz.Spi;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Quartz
{
    //Necessary to inject dependencies in job, using factory allows DI container to take charge of creating quartz jobs and handling dependencies
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (_serviceProvider.GetService(bundle.JobDetail.JobType) as IJob)!;
        }

        //Quartz handles exceptions internally and calls this method to dispose of job when exception is thrown inside job 
        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
