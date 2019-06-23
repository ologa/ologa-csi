using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;

namespace EFDataAccess.Tasks
{
    public class SchedulerFactory
    {
        public async System.Threading.Tasks.Task InitializeAsync()
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection {{ "quartz.serializer.type", "binary" }};
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ReportGenerator>()
                .WithIdentity("ReportGenerator", "Reports")
                .Build();

            // In a near future this queries should come from the DB
            VPPS.CSI.Domain.Query query = new VPPS.CSI.Domain.Query();
            query.Sql = @"select Username, FirstName, LastName, LastLoginDate, DefSite from [User]";
            query.Code = "user-search";
            query.Name = "Search for User information";

            sched.Context.Add("query", query);

            // Trigger the job to run now, and then every 40 seconds
            //ITrigger trigger = TriggerBuilder.Create()
            // .WithIdentity("ReportGenerator", "Reports")
            //.StartAt(DateBuilder.DateOf(0, 0, 0, 13, 12, 2018)).WithSchedule(onMondayAndTuesday)
            //.StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(120).RepeatForever()).Build();

            //await sched.ScheduleJob(job, trigger);

            //// Trigger the job to run now, and then every 40 seconds
            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("ReportGenerator", "Reports")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x.WithIntervalInMinutes(2).RepeatForever()).Build();

            //// Tell quartz to schedule the job using our trigger
            //await sched.ScheduleJob(job, trigger);


            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("ReportGenerator", "Reports")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(40).RepeatForever()).Build();

            //await sched.ScheduleJob(job, trigger);

        }
    }
}
