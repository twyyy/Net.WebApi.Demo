using Microsoft.Extensions.DependencyInjection;
using Net.WebApi.Demo.Common.OtherExts;
using System.Reflection;
using System.Runtime.Loader;

namespace Net.WebApi.Demo.Common.AutoDiExts;

/// <summary>
/// 自动注册到DI容器扩展
/// </summary>
public static class AutoDi
{
    /// <summary>
    /// 自动注册到DI容器
    /// </summary>
    /// <param name="service"></param>
    public static void AddAutoDi(this IServiceCollection service)
    {
        // 获取配置的程序集, 如果为空则不处理
        var dlls = ConfigExt.Get<List<string>>("Initial:AutoDiDlls") ?? new List<string>();
        if (!dlls.Any())
        {
            return;
        }

        // 获取所有自动依赖注入的类型并整合
        var types = dlls.AsParallel()
            .Select(dll => new AssemblyLoadContext(null, true).LoadFromAssemblyPath($"{dll}.dll").GetTypes())
            .SelectMany(types => types)
            .Where(type => type.IsClass && type.GetCustomAttribute<AutoDiAttribute>() != null)
            .ToList();

        // 遍历
        Parallel.ForEach(types, type =>
        {
            // 获取Attr并从Attr中获取生命周期和接口注册选项
            var autoDi = type.GetCustomAttribute<AutoDiAttribute>()!;
            var lifeCycleEnum = autoDi.LifeCycleEnum;
            var notUseInterface = autoDi.NotUseInterface;

            // 获取接口
            var interfaces = type.GetInterfaces();

            // 如果接口不为空, 并且允许使用接口注册
            if (interfaces.Any() && !notUseInterface)
            {
                // 遍历接口
                foreach (var item in interfaces)
                {
                    switch (lifeCycleEnum)
                    {
                        case LifeCycleEnum.Singleton:
                            service.AddSingleton(item, type);
                            break;
                        case LifeCycleEnum.Scoped:
                            service.AddScoped(item, type);
                            break;
                        case LifeCycleEnum.Transient:
                            service.AddTransient(item, type);
                            break;
                        default:
                            break;
                    }
                }

                return;
            }

            switch (lifeCycleEnum)
            {
                case LifeCycleEnum.Singleton:
                    service.AddSingleton(type);
                    break;
                case LifeCycleEnum.Scoped:
                    service.AddScoped(type);
                    break;
                case LifeCycleEnum.Transient:
                    service.AddTransient(type);
                    break;
                default:
                    break;
            }
        });
    }
}