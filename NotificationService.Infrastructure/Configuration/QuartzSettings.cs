using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Configuration
{
    public class QuartzSettings
    {
        public JobsSettings Jobs { get; set; } = new JobsSettings();

        public static TimeSpan GetTimeSpanFromInterval(IntervalSettings interval)
        {
            return interval.Unit switch
            {
                "Milliseconds" => TimeSpan.FromMilliseconds(interval.Value),
                "Seconds" => TimeSpan.FromSeconds(interval.Value),
                "Minutes" => TimeSpan.FromMinutes(interval.Value),
                "Hours" => TimeSpan.FromHours(interval.Value),
                "Days" => TimeSpan.FromDays(interval.Value),
                _ => throw new NotSupportedException($"Unsupported unit: {interval.Unit}")
            };
        }
    }

    public class JobsSettings
    {
        public NotificationCleanupSettings NotificationCleanup { get; set; } = new NotificationCleanupSettings();
    }

    /// <summary>
    /// Configuration settings for the notification cleanup job.
    /// </summary>
    public class NotificationCleanupSettings
    {
        /// <summary>
        /// The interval at which the notification cleanup job is scheduled to run.
        /// Specifies how frequently the Quartz scheduler should trigger the cleanup job.
        /// </summary>
        public IntervalSettings JobRunningInterval { get; set; } = new IntervalSettings();

        /// <summary>
        /// The retention period for notifications before they are eligible for deletion.
        /// Specifies how long notifications are retained from their creation date.
        /// </summary>
        public IntervalSettings NotificationRetentionPeriod { get; set; } = new IntervalSettings();
    }

    /// <summary>
    /// Represents a configurable interval with a value and a unit.
    /// </summary>
    public class IntervalSettings
    {
        /// <summary>
        /// The interval value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The unit of the interval ("Milliseconds", "Seconds", "Minutes", "Hours", "Days").
        /// </summary>
        public string Unit { get; set; } = "Days";
    }
}
