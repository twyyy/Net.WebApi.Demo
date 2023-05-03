using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Net.WebApi.Demo.Common.Attrs;
using Net.WebApi.Demo.Common.Mongo;
using Net.WebApi.Demo.Model.Mongo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Net.WebApi.Demo.Repository.MongoDB;

/// <summary>
/// Mongo仓储抽象
/// </summary>
public abstract class MongoBaseRep<T> : IMongoBaseRep<T> where T : MongoBaseEntity
{
    private readonly string CollectionName;

    private readonly IMongoCollection<T> Db;

    protected MongoBaseRep()
    {
        // 获取集合名称
        var tableAttr = Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute)) as TableAttribute;
        var collectionName = tableAttr?.Name ?? typeof(T).Name;
        CollectionName = collectionName;

        // 获取数据库名称
        var dataBaseAttr = Attribute.GetCustomAttribute(typeof(T), typeof(DbNameAttribute)) as DbNameAttribute;
        var dataBaseName = dataBaseAttr?._name ?? "";

        Db = Mongo.GetCollection<T>(collectionName, dataBaseName);
    }

    /// <summary>
    /// 查询是否存在
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task<bool> AnyAsync(Expression<Func<T, bool>> condition, bool containsDel = false)
    {
        var filter = containsDel
            ? Builders<T>.Filter.And(
                Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.Where(x => x.DelTime == null)
              )
            : Builders<T>.Filter.Where(condition);
        return Db.Find(filter).AnyAsync();
    }

    /// <summary>
    /// 查询数量
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task<long> CountAsync(Expression<Func<T, bool>> condition, bool containsDel = false)
    {
        var filter = containsDel
            ? Builders<T>.Filter.And(
                Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.Where(x => x.DelTime == null)
              )
            : Builders<T>.Filter.Where(condition);
        return Db.Find(filter).CountDocumentsAsync();
    }

    /// <summary>
    /// 条件删除 (假)
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task DeleteCondAsync(Expression<Func<T, bool>> condition, bool containsDel = false)
    {
        var filter = containsDel
            ? Builders<T>.Filter.And(
                Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.Where(x => x.DelTime == null)
              )
            : Builders<T>.Filter.Where(condition);
        var update = Builders<T>.Update.Set(x => x.DelTime, DateTime.Now);
        return Db.UpdateManyAsync(filter, update);
    }

    /// <summary>
    /// 删除多条数据 (假)
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public Task DeleteManyAsync(IEnumerable<T> docs)
    {
        var ids = docs.AsParallel().Select(x => x.Id).ToHashSet();
        return DeleteCondAsync(x => ids.Contains(x.Id));
    }

    /// <summary>
    /// 删除一条数据 (假)
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task DeleteOneAsync(T doc, bool containsDel = false)
    {
        var filter = containsDel
            ? Builders<T>.Filter.And(
                Builders<T>.Filter.Where(x => x.Id == doc.Id),
                Builders<T>.Filter.Where(x => x.DelTime == null)
              )
            : Builders<T>.Filter.Where(x => x.Id == doc.Id);
        var update = Builders<T>.Update.Set(x => x.DelTime, DateTime.Now);
        return Db.UpdateOneAsync(filter, update);
    }

    /// <summary>
    /// 查找一条数据
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> condition, bool containsDel = false)
    {
        var filter = containsDel
            ? Builders<T>.Filter.And(
                Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.Where(x => x.DelTime == null)
              )
            : Builders<T>.Filter.Where(condition);
        return Db.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// 获取集合名称
    /// </summary>
    /// <returns></returns>
    public string GetCollectionName()
    {
        return CollectionName;
    }

    /// <summary>
    /// 插入多条数据
    /// </summary>
    /// <param name="docs"></param>
    /// <returns></returns>
    public Task InsertManyAsync(IEnumerable<T> docs)
    {
        return Db.InsertManyAsync(docs);
    }

    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public Task InsertOneAsync(T doc)
    {
        return Db.InsertOneAsync(doc);
    }

    /// <summary>
    /// 替换多条数据
    /// </summary>
    /// <param name="docs"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task ReplaceManyAsync(IEnumerable<T> docs, bool containsDel = false)
    {
        var replacements = docs.AsParallel().Select(x =>
        {
            var filter = containsDel
                ? Builders<T>.Filter.And(
                    Builders<T>.Filter.Where(w => w.Id == x.Id),
                    Builders<T>.Filter.Where(x => x.DelTime == null)
                  )
                : Builders<T>.Filter.Where(w => w.Id == x.Id);
            return new ReplaceOneModel<T>(filter, x);
        });
        
        return Db.BulkWriteAsync(replacements);
    }

    /// <summary>
    /// 替换一条数据
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task ReplaceOneAsync(T doc, bool containsDel = false)
    {
        var filter = containsDel
                ? Builders<T>.Filter.And(
                    Builders<T>.Filter.Where(w => w.Id == doc.Id),
                    Builders<T>.Filter.Where(x => x.DelTime == null)
                  )
                : Builders<T>.Filter.Where(w => w.Id == doc.Id);
        return Db.ReplaceOneAsync(filter, doc);
    }

    /// <summary>
    /// 查找多条数据
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task<List<T>> ToListAsync(Expression<Func<T, bool>> condition, bool containsDel = false)
    {
        var filter = containsDel
            ? Builders<T>.Filter.And(
                Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.Where(x => x.DelTime == null)
              )
            : Builders<T>.Filter.Where(condition);
        return Db.Find(filter).ToListAsync();
    }

    /// <summary>
    /// 条件更新
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="updateDefinition"></param>
    /// <param name="containsDel"></param>
    /// <returns></returns>
    public Task UpdateCondAsync(Expression<Func<T, bool>> condition, UpdateDefinition<T> updateDefinition, bool containsDel = false)
    {
        var filter = containsDel
            ? Builders<T>.Filter.And(
                Builders<T>.Filter.Where(condition),
                Builders<T>.Filter.Where(x => x.DelTime == null)
              )
            : Builders<T>.Filter.Where(condition);

        return Db.UpdateManyAsync(filter, updateDefinition);
    }

    /// <summary>
    /// 更新多条数据
    /// </summary>
    /// <param name="updateDefinition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task UpdateManyAsync(Dictionary<T, UpdateDefinition<T>> updateDefinition, bool containsDel = false)
    {

        var updates = updateDefinition.AsParallel().Select(x =>
        {
            var filter = containsDel
                ? Builders<T>.Filter.And(
                    Builders<T>.Filter.Where(w => w.Id == x.Key.Id),
                    Builders<T>.Filter.Where(x => x.DelTime == null)
                  )
                : Builders<T>.Filter.Where(w => w.Id == x.Key.Id);
            return new UpdateOneModel<T>(filter, x.Value);
        });

        return Db.BulkWriteAsync(updates);
    }

    /// <summary>
    /// 更新一条数据
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="updateDefinition"></param>
    /// <param name="containsDel">是否包含假删除的数据</param>
    /// <returns></returns>
    public Task UpdateOneAsync(T doc, UpdateDefinition<T> updateDefinition, bool containsDel = false)
    {
        var filter = containsDel
                ? Builders<T>.Filter.And(
                    Builders<T>.Filter.Where(w => w.Id == doc.Id),
                    Builders<T>.Filter.Where(x => x.DelTime == null)
                  )
                : Builders<T>.Filter.Where(w => w.Id == doc.Id);

        return Db.UpdateOneAsync(filter, updateDefinition);
    }
}
