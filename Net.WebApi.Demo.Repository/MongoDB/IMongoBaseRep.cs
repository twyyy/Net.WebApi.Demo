using MongoDB.Driver;
using Net.WebApi.Demo.Model.Mongo;
using System.Linq.Expressions;

namespace Net.WebApi.Demo.Repository.MongoDB;

/// <summary>
/// Mongo仓储接口
/// </summary>
public interface IMongoBaseRep<T> where T : MongoBaseEntity
{
    /// <summary>
    /// 获取集合名称
    /// </summary>
    /// <returns></returns>
    string GetCollectionName();

    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    Task InsertOneAsync(T doc);

    /// <summary>
    /// 插入多条数据
    /// </summary>
    /// <param name="docs"></param>
    /// <returns></returns>
    Task InsertManyAsync(IEnumerable<T> docs);

    /// <summary>
    /// 删除一条数据 (假)
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task DeleteOneAsync(T doc, bool containsDel = false);

    /// <summary>
    /// 删除多条数据 (假)
    /// </summary>
    /// <param name="docs"></param>
    /// <returns></returns>
    Task DeleteManyAsync(IEnumerable<T> docs);

    /// <summary>
    /// 条件删除 (假)
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task DeleteCondAsync(Expression<Func<T, bool>> condition, bool containsDel = false);

    /// <summary>
    /// 替换一条数据
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task ReplaceOneAsync(T doc, bool containsDel = false);

    /// <summary>
    /// 替换多条数据
    /// </summary>
    /// <param name="docs"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task ReplaceManyAsync(IEnumerable<T> docs, bool containsDel = false);

    /// <summary>
    /// 更新一条数据
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="updateDefinition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task UpdateOneAsync(T doc, UpdateDefinition<T> updateDefinition, bool containsDel = false);

    /// <summary>
    /// 更新多条数据
    /// </summary>
    /// <param name="updateDefinition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task UpdateManyAsync(Dictionary<T, UpdateDefinition<T>> updateDefinition, bool containsDel = false);

    /// <summary>
    /// 条件更新
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="updateDefinition"></param>
    /// <param name="containsDel"></param>
    /// <returns></returns>
    Task UpdateCondAsync(Expression<Func<T, bool>> condition, UpdateDefinition<T> updateDefinition, bool containsDel = false);

    /// <summary>
    /// 查找一条数据
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> condition, bool containsDel = false);

    /// <summary>
    /// 查找多条数据
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task<List<T>> ToListAsync(Expression<Func<T, bool>> condition, bool containsDel = false);

    /// <summary>
    /// 查询数量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task<long> CountAsync(Expression<Func<T, bool>> condition, bool containsDel = false);

    /// <summary>
    /// 查询是否存在
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> condition, bool containsDel = false);
}