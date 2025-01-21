using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotificationService.Infrastructure.Configuration;
using NotificationService.Infrastructure.Quartz;
using Quartz;
using Quartz.Spi;

namespace NotificationService.Tests.Integration.Helpers
{
    public class TestQuartzScheduler : QuartzScheduler
    {
        private readonly TaskCompletionSource<bool> _jobCompletionSource;

        public TestQuartzScheduler(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IOptions<QuartzSettings> quartzSettings)
            : base(schedulerFactory, jobFactory, quartzSettings)
        {
            _jobCompletionSource = new TaskCompletionSource<bool>();        
        }

        public void AddTriggerListenerToScheduler(int totalJobs)
        {
            var triggerListener = new JobCompletionTriggerListener(_jobCompletionSource, totalJobs);

            if (_scheduler != null)
            {
                _scheduler.ListenerManager.AddTriggerListener(triggerListener);
            }
            else
            {
                throw new Exception("The scheduler instance is null. Ensure that the scheduler has been properly initialized before use.");
            }
        }

        // Expose a task for tests to await until the job completes
        public Task JobCompletion => _jobCompletionSource.Task;
    }

    public class JobCompletionTriggerListener : ITriggerListener
    {
        private readonly TaskCompletionSource<bool> _jobCompletionSource;
        private int _remainingJobs;

        public JobCompletionTriggerListener(TaskCompletionSource<bool> jobCompletionSource, int totalJobs)
        {
            _jobCompletionSource = jobCompletionSource;
            _remainingJobs = totalJobs;
        }

        public string Name => "JobCompletionTriggerListener";

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            if (Interlocked.Decrement(ref _remainingJobs) == 0)
            {
                _jobCompletionSource.SetResult(true);
            }
            return Task.CompletedTask;
        }
    }
}
