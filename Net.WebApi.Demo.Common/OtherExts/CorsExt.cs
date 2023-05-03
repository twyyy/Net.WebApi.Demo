using Microsoft.Extensions.DependencyInjection;

namespace Net.WebApi.Demo.Common.OtherExts;

/// <summary>
/// 跨域扩展
/// </summary>
public static class CorsExt
{
    /// <summary>
    /// 添加跨域规则
    /// </summary>
    /// <param name="service">服务对象</param>
    /// <returns></returns>
    public static void AddCorsPolicy(this IServiceCollection service)
    {
        // 自动添加跨域配置
        service.AddCors(options =>
        {
            // 获取跨域配置
            var dict = ConfigExt.Get<Dictionary<string, string>>("Cors") ??
                       new Dictionary<string, string>();

            // 循环添加跨域配置
            Parallel.ForEach(dict, item =>
            {
                options.AddPolicy(item.Key, policy =>
                {
                    policy.WithOrigins(item.Value.Split(","));
                });
            });
        });
    }
}