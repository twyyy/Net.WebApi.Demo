namespace Net.WebApi.Demo.Common.AutoDiExts;

/// <summary>
/// 自动依赖注入生命周期
/// </summary>
public enum LifeCycleEnum
{
    /// <summary>
    /// 单例
    /// </summary>
    Singleton,
    /// <summary>
    /// 作用域
    /// </summary>
    Scoped,
    /// <summary>
    /// 瞬时
    /// </summary>
    Transient
}