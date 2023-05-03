using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Runtime.Loader;

namespace Net.WebApi.Demo.Common.OtherExts;

/// <summary>
/// 中间件扩展
/// </summary>
public static class MiddlewareExt
{
    /// <summary>
    /// 使用自定义中间件
    /// </summary>
    /// <param name="app"></param>
    public static void UseCustomMiddleware(this IApplicationBuilder app)
    {
        // 获取配置的程序集, 如果为空则不处理
        var dlls = ConfigExt.Get<List<string>>("Middleware:Dlls") ?? new List<string>();
        if (!dlls.Any())
        {
            return;
        }

        // 获取所有中间件类型并整合
        var types = dlls.AsParallel()
            .Select(dll => new AssemblyLoadContext(null, true).LoadFromAssemblyPath($"{dll}.dll").GetTypes())
            .SelectMany(types => types)
            .Where(type => type.IsClass && type.GetInterface(nameof(IMiddleware)) != null)
            .ToList();

        // 读取中间件权重配置
        var dict = ConfigExt.Get<Dictionary<string, int>>("Middleware:Sort") ??
                   new Dictionary<string, int>();

        // 过滤掉未配置权重的中间件, 并且按照权重排序
        var sortTypes = types.AsParallel().Select(type =>
        {
            // 判断是否配置权重
            if (!dict.ContainsKey(type.Name))
            {
                return null;
            }

            // 获取权重并返回
            var sort = dict[type.Name];
            return new { type, sort };
        }).Where(i => i != null).OrderByDescending(i => i!.sort).ToList();

        // 循环添加中间件
        foreach (var item in sortTypes)
        {
            app.UseMiddleware(item!.type);
        }
    }
}