namespace Net.WebApi.Demo.Common.WebServiceExts;

/// <summary>
/// 标记webservice接口
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class WebServiceAttribute : Attribute
{
}
