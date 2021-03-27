using Hangfire;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Jobs
{
    public interface IMyJob
    {
        Task RunFirstJob(IJobCancellationToken token);
        Task RunMarketsReport(DateTime now);
    }

    public class MyJob : IMyJob
    {
        public async Task RunFirstJob(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunMarketsReport(DateTime.Now);

        }

        [JobDisplayName("RunJobLocalWriteWord")]
        public async Task RunMarketsReport(DateTime now)
        {
             Debug.WriteLine("===HERE===");
        }

        public interface IHangfireJobScheduler
        {
            void ScheduleRecurringJobs();
        }

        public class HangfireJobScheduler : IHangfireJobScheduler
        {
            [Obsolete]
            public void ScheduleRecurringJobs()
            {
                RecurringJob.RemoveIfExists(nameof(MyJob));
                RecurringJob.AddOrUpdate<MyJob>(nameof(MyJob),
                    job => job.RunFirstJob(JobCancellationToken.Null),
                    Cron.MinuteInterval(1), TimeZoneInfo.Utc);
            }
        }


    }
}

