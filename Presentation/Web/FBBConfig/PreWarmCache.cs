using Quartz;
using Quartz.Spi;
using System;

namespace FBBConfig
{
    using FBBConfig.Jobs;
    using FBBConfig.Solid.CompositionRoot;
    using WBBBusinessLayer;

    public class PreWarmCache : System.Web.Hosting.IProcessHostPreloadClient
    {
        public ILogger _logger { get; set; }

        private static IScheduler _sched;
        private static IJobDetail _runAtMorningJob;
        private static IJobDetail _runAtAfternoonJob;
        private static IJobDetail _webPingJob;

        private static ITrigger _runAtMorningTrigger;
        private static ITrigger _runAtAfternoonTrigger;
        private static ITrigger _webPingTrigger;

        public void Preload(string[] parameters)
        {
            _logger = Bootstrapper.GetInstance<DebugLogger>();

            _sched = Bootstrapper.GetInstance<IScheduler>();
            _sched.JobFactory = Bootstrapper.GetInstance<IJobFactory>();
            _sched.Start();

            QuartzConfig();

            _sched.ScheduleJob(_runAtMorningJob, _runAtMorningTrigger);
            //_sched.ScheduleJob(_runAtAfternoonJob, _runAtAfternoonTrigger);
            _sched.ScheduleJob(_webPingJob, _webPingTrigger);

        }

        private void QuartzConfig()
        {
            try
            {
                QuartzNetWebConsole.Setup.Scheduler = () => _sched;

                // morning job
                _runAtMorningJob = JobBuilder.Create<QueryBuildingVillage>()
                    .WithIdentity("runAtMorningJob", "morningGroup")
                    .Build();

                _runAtMorningTrigger = TriggerBuilder.Create()
                    .WithIdentity("runAtMorningTrigger", "morningGroup")
                    .WithSchedule(CronScheduleBuilder
                                    .DailyAtHourAndMinute(5, 00)
                                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                                    .WithMisfireHandlingInstructionDoNothing())
                    .Build();

                // afternoon job
                //_runAtAfternoonJob = JobBuilder.Create<QueryBuildingVillage>()
                //   .WithIdentity("runAtAfternoonJob", "afternoonGroup")
                //   .Build();

                //_runAtAfternoonTrigger = TriggerBuilder.Create()
                //   .WithIdentity("runAtAfternoonTrigger", "afternoonGroup")
                //   .WithSchedule(CronScheduleBuilder
                //                    .DailyAtHourAndMinute(13, 12)
                //                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                //                    .WithMisfireHandlingInstructionDoNothing())
                //   .Build();

                // web ping job
                _webPingJob = JobBuilder.Create<WebPing>()
                   .WithIdentity("webPingJob", "webPingGroup")
                   .Build();

                _webPingTrigger = TriggerBuilder.Create()
                   .WithIdentity("webPingTrigger ", "webPingGroup")
                   .StartNow()
                   .WithSimpleSchedule(x => x
                       .WithIntervalInMinutes(5)
                       .RepeatForever())
                   .Build();

            }
            catch (SchedulerException ex)
            {
                _logger.Info("SchedulerException : " + (ex.InnerException == null ? ex.Message : ex.InnerException.Message));
            }
        }

    }
}