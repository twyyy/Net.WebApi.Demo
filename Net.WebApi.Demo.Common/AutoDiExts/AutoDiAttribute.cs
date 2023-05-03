namespace Net.WebApi.Demo.Common.AutoDiExts;

/// <summary>
/// 自动依赖注入属性配置
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutoDiAttribute : Attribute
{
    /// <summary>
    /// 生命周期
    /// </summary>
    public readonly LifeCycleEnum LifeCycleEnum;

    /// <summary>
    /// 不使用接口, 直接注册
    /// </summary>
    public readonly bool NotUseInterface;

    /// <summary>
    /// 初始化生命周期
    /// </summary>
    public AutoDiAttribute(LifeCycleEnum lifeCycleEnum = LifeCycleEnum.Scoped, bool notUseInterface = false)
    {
        LifeCycleEnum = lifeCycleEnum;
        NotUseInterface = notUseInterface;
    }
}