using MongoDB.Driver;
using Net.WebApi.Demo.Common.OtherExts;

namespace Net.WebApi.Demo.Common.MongoExts;

/// <summary>
/// mongodb数据库
/// </summary>
public static class Mongo
{
    /// <summary>
    /// 懒加载静态单例
    /// </summary>
    private static readonly Lazy<Dictionary<string, IMongoDatabase>> DbList = new(() =>
    {
        // 获取配置
        var configs = ConfigExt.Get<Dictionary<string, string>>("Mongo") ?? new Dictionary<string, string>();

        // 创建连接
        return configs.AsParallel()
            .Select(w => new KeyValuePair<string, IMongoDatabase>(w.Key, new MongoClient(w.Value).GetDatabase(w.Key)))
            .ToDictionary(k => k.Key, v => v.Value);
    });

    /// <summary>
    /// 获取mongodb数据库集合
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="dataBaseName"></param>
    /// <returns></returns>
    public static IMongoCollection<T> GetCollection<T>(string collectionName, string? dataBaseName = null)
        where T : class
    {
        // 判断是否传入数据库名称, 如果没有传入则使用默认数据库
        if (string.IsNullOrWhiteSpace(dataBaseName))
        {
            return DbList.Value.FirstOrDefault().Value.GetCollection<T>(collectionName);
        }

        return DbList.Value[dataBaseName].GetCollection<T>(collectionName);
    }
}