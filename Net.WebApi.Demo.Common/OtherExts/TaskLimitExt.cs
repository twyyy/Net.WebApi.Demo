namespace Net.WebApi.Demo.Common.OtherExts;

/// <summary>
/// task任务限制
/// </summary>
public static class TaskLimitExt
{
    /// <summary>
    /// 懒加载静态单例
    /// </summary>
    private static readonly Lazy<SemaphoreSlim> SemaphoreLazy = new(() =>
    {
        // 获取配置
        var limit = ConfigExt.Get<int?>("Initial:TaskLimit") ?? int.MaxValue;

        // 如果限制值小于等于0, 则使用逻辑处理器的数量作为限制值
        limit = limit switch
        {
            <= 0 => Environment.ProcessorCount,
            _ => limit
        };

        return new SemaphoreSlim(limit);
    });
    public static SemaphoreSlim Semaphore => SemaphoreLazy.Value;
}