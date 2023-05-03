using Microsoft.Extensions.Configuration;

namespace Net.WebApi.Demo.Common.OtherExts;

/// <summary>
/// 系统配置扩展
/// </summary>
public static class ConfigExt
{
    /// <summary>
    /// 系统配置接口对象
    /// </summary>
    private static IConfiguration? _configuration;

    /// <summary>
    /// 初始化锁, 防止多次初始化
    /// </summary>
    private static readonly object LockObj = new();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="configuration">系统配置接口对象</param>
    public static void InitAppSettings(this IConfiguration configuration)
    {
        // 加锁, 防止多次初始化
        lock (LockObj)
        {
            // 如果不等于null, 表示已经初始化, 不再初始化
            if (_configuration != null)
            {
                return;
            }

            // 初始化
            _configuration = configuration;
        }
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="key">读取配置的KEY</param>
    /// <returns></returns>
    public static T? Get<T>(string key) => _configuration == null ? default : _configuration.GetSection(key).Get<T>();
}