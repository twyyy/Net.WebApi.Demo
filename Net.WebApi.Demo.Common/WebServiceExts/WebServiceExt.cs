using Microsoft.AspNetCore.Builder;
using System.Reflection;
using SoapCore;
using Net.WebApi.Demo.Common.OtherExts;
using System.Runtime.Loader;

namespace Net.WebApi.Demo.Common.WebServiceExts;

/// <summary>
/// webservice扩展
/// </summary>
public static class WebServiceExt
{
    /// <summary>
    /// 使用webservice
    /// </summary>
    /// <param name="app"></param>
    public static void UseWebService(this WebApplication app)
    {
        // 获取配置的程序集, 如果为空则不处理
        var dlls = ConfigExt.Get<List<string>>("WebService:Dlls") ?? new List<string>();
        if (!dlls.Any())
        {
            return;
        }

        // 获取所有自动依赖注入的类型并整合
        var types = dlls.AsParallel()
            .Select(dll => new AssemblyLoadContext(null, true).LoadFromAssemblyPath($"{dll}.dll").GetTypes())
            .SelectMany(types => types)
            .Where(type => type.IsInterface && type.GetCustomAttribute<WebServiceAttribute>() != null)
            .ToList();

        // 遍历使用webservice
        Parallel.ForEach(types, type =>
        {
            // 获取配置的地址, 如果未配置则不使用
            var address = ConfigExt.Get<string>($"WebService:Enable:{type.Name}");
            if (string.IsNullOrWhiteSpace(address))
            {
                return;
            }

            app.UseSoapEndpoint(type, address, new SoapEncoderOptions());
        });
    }
}