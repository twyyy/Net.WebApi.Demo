using Microsoft.Extensions.Caching.Memory;

namespace Net.WebApi.Demo.Common.CacheExts;

/// <summary>
/// 本地缓存
/// </summary>
public class Cache
{
    /// <summary>
    /// 懒加载静态单例
    /// </summary>
    private static readonly Lazy<MemoryCache> MCache = new(() => new MemoryCache(new MemoryCacheOptions()));

    /// <summary>
    /// 清空缓存
    /// </summary>
    public static void Clear() => MCache.Value.Clear();

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Set<T>(string key, T value) => MCache.Value.Set(key, value);

    /// <summary>
    /// 设置缓存, 并设置绝对过期时间
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="time"></param>
    public static void Set<T>(string key, T value, DateTimeOffset time) => MCache.Value.Set(key, value, time);

    /// <summary>
    /// 设置缓存, 并设置相对过期时间
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="time"></param>
    public static void Set<T>(string key, T value, TimeSpan time) => MCache.Value.Set(key, value, time);

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key"></param>
    public static void Remove(string key) => MCache.Value.Remove(key);

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    public static T? Get<T>(string key) => MCache.Value.Get<T>(key);
}