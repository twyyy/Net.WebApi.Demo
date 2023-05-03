namespace Net.WebApi.Demo.Common.MongoExts;

/// <summary>
/// 数据库名称指定属性
/// </summary>
[AttributeUsage(AttributeTargets.Class,AllowMultiple = false, Inherited = false)]
public class DbNameAttribute : Attribute
{
    /// <summary>
    /// 数据库名称
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 初始化数据库名称
    /// </summary>
    /// <param name="name"></param>
    public DbNameAttribute(string name)
    {
        Name = name;
    }
}