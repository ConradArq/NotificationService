using Microsoft.Extensions.Options;
using Quartz.Spi;
using Quartz;
using NotificationService.Infrastructure.Quartz.Jobs;
using NotificationService.Infrastructure.Configuration;

namespace NotificationService.Infrastructure.Quartz
{
    public class QuartzScheduler
    {
        protected IScheduler? _scheduler;

        protected readonly ISchedulerFactory _schedulerFactory;
        protected readonly IJobFactory _jobFactory;
        private readonly QuartzSettings _quartzSettings;

        public QuartzScheduler(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IOptions<QuartzSettings> quartzSettings)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _quartzSettings = quartzSettings.Value;
        }

        public async Task StartAsync()
        {
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _jobFactory;
            await _scheduler.Start();
        }

        public async Task InitJobs()
        {
            TimeSpan? notificationCleanupJobRunningInterval = null;

            if (_quartzSettings.Jobs.NotificationCleanup.JobRunningInterval.Value > 0)
            {
                notificationCleanupJobRunningInterval = QuartzSettings.GetTimeSpanFromInterval(_quartzSettings.Jobs.NotificationCleanup.JobRunningInterval);
            }

            await ScheduleNotificationsCleanUp(notificationCleanupJobRunningInterval);
        }

        /// <summary>
        /// Schedules the cleaning of notifications at intervals defined by <paramref name="jobRunningInterval"/>.
        /// <param name="jobRunningInterval">The interval at which the notification cleanup job should run.</param>
        /// </summary>
        public async Task ScheduleNotificationsCleanUp(TimeSpan? jobRunningInterval)
        {
            var jobKey = new JobKey("Notification", "CleanUp");
            var jobDataMap = new JobDataMap();
            await ScheduleJob<NotificationCleanUpJob>(jobKey, jobDataMap, DateTime.UtcNow, jobRunningInterval);
        }

        /// <summary>
        /// Schedules a job with the specified key, data, start time, and optional interval.
        /// If an interval is provided, the job will execute repeatedly at the specified interval.
        /// Otherwise, the job will execute only once at the specified start time.
        /// </summary>
        /// <typeparam name="T">The type of job to schedule, implementing <see cref="IJob"/>.</typeparam>
        /// <param name="jobKey">The unique key identifying the job.</param>
        /// <param name="jobDataMap">The data map containing parameters for the job.</param>
        /// <param name="jobStartTimeUtc">The time in UTC when the job is scheduled to start.</param>
        /// <param name="interval">The optional interval for recurring job execution.</param>
        private async Task ScheduleJob<T>(
            JobKey jobKey,
            JobDataMap jobDataMap,
            DateTime jobStartTimeUtc,
            TimeSpan? interval = null) where T : IJob
        {
            if (_scheduler != null)
            {
                if (await _scheduler.CheckExists(jobKey))
                {
                    await _scheduler.DeleteJob(jobKey);
                }

                var job = JobBuilder.Create<T>()
                    .WithIdentity(jobKey)
                    .UsingJobData(jobDataMap)
                    .Build();

                var triggerKey = new TriggerKey(jobKey.Name + "_trigger", jobKey.Group);

                ITrigger trigger;

                if (interval.HasValue)
                {
                    trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .StartAt(jobStartTimeUtc)
                        .WithSimpleSchedule(x => x
                            .WithInterval(interval.Value)
                            .RepeatForever())
                        .ForJob(job)
                        .Build();
                }
                else
                {
                    trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .StartAt(jobStartTimeUtc)
                        .ForJob(job)
                        .Build();
                }

                await _scheduler.ScheduleJob(job, trigger);
            }
            else
            {
                throw new Exception("The scheduler instance is null. Ensure that the scheduler has been properly initialized before use.");
            }
        }

        public async Task ShutdownAsync()
        {
            if (_scheduler != null)
            {
                await _scheduler.Shutdown();
            }
            else
            {
                throw new Exception("The scheduler instance is null. Ensure that the scheduler has been properly initialized before use.");
            }
        }
    }
}
