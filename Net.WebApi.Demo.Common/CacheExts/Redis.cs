using Net.WebApi.Demo.Common.OtherExts;
using StackExchange.Redis;

namespace Net.WebApi.Demo.Common.CacheExts;

/// <summary>
/// redis 缓存
/// </summary>
public class Redis
{
    /// <summary>
    /// 懒加载静态单例
    /// </summary>
    private static readonly Lazy<List<ConnectionMultiplexer>> Clients = new(() =>
    {
        // 获取配置
        var redisConfigs = ConfigExt.Get<List<string>>("Redis") ?? new List<string>();

        // 创建连接
        return redisConfigs.AsParallel().Select(w => ConnectionMultiplexer.Connect(w)).ToList();
    });

    /// <summary>
    /// 获取redis连接
    /// </summary>
    /// <param name="index">目标连接在配置中的索引位置</param>
    /// <returns></returns>
    public static ConnectionMultiplexer? GetConnection(int index = 0)
    {
        // 如果连接池为空, 则返回null
        if (!Clients.Value.Any())
        {
            return null;
        }

        // 判断索引是否越界, 如果越界则抛出错误
        if (index < 0 || index > Clients.Value.Count - 1)
        {
            throw new IndexOutOfRangeException($"索引 {index} 越界");
        }

        // 返回目标连接
        return Clients.Value[index];
    }

    /// <summary>
    /// 获取redis数据库
    /// </summary>
    /// <param name="index">目标连接在配置中的索引位置. 数据库编号使用配置的编号, 未配置默认0</param>
    /// <returns></returns>
    public static IDatabase? GetDatabase(int index = 0)
    {
        // 如果连接池为空, 则返回null
        if (!Clients.Value.Any())
        {
            return null;
        }

        // 判断索引是否越界, 如果越界则抛出错误
        if (index < 0 || index > Clients.Value.Count - 1)
        {
            throw new IndexOutOfRangeException($"索引 {index} 越界");
        }

        return Clients.Value[index].GetDatabase();
    }
}