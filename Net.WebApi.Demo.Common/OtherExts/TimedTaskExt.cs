using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Runtime.Loader;

namespace Net.WebApi.Demo.Common.OtherExts;

/// <summary>
/// 定时任务扩展
/// </summary>
public static class TimedTaskExt
{
    /// <summary>
    /// 添加定时任务
    /// </summary>
    /// <param name="service">服务对象</param>
    public static void AddTimedTask(this IServiceCollection service)
    {
        // 自动添加定时任务
        service.AddQuartz(q =>
        {
            // 任务对象接入依赖注入功能
            q.UseMicrosoftDependencyInjectionJobFactory();

            // 获取配置的程序集, 如果为空则不处理
            var dlls = ConfigExt.Get<List<string>>("TimedTask:Dlls") ?? new List<string>();
            if (!dlls.Any())
            {
                return;
            }

            // 获取所有定时任务类并整合
            var types = dlls.AsParallel()
                .Select(dll => new AssemblyLoadContext(null, true).LoadFromAssemblyPath($"{dll}.dll").GetTypes())
                .SelectMany(types => types)
                .Where(type => type.IsClass && type.GetInterface(nameof(IJob)) != null)
                .ToList();

            // 循环添加定时任务
            Parallel.ForEach(types, type =>
            {
                // 获取配置的Cron表达式
                var cron = ConfigExt.Get<string>($"TimedTask:Cron:{type.Name}");
                if (string.IsNullOrEmpty(cron))
                {
                    return;
                }

                // 添加定时任务
                var jobKey = new JobKey(type.Name);
                q.AddJob(type, jobKey, opts => opts.WithIdentity(jobKey));

                // 定时任务触发器
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{type.Name}-trigger")
                    .WithCronSchedule(cron)
                );
            });
        });

        // 将 Quartz.NET 调度器作为 ASP.NET Core 的后台服务 
        // 应用程序关闭时，调度器会等待所有正在执行的作业完成后再停止
        service.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}