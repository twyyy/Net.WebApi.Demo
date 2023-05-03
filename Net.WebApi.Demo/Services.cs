using Net.WebApi.Demo.Common.AutoDiExts;
using Net.WebApi.Demo.Common.OtherExts;
using Net.WebApi.Demo.Common.SwaggerExts;
using SoapCore;

namespace Net.WebApi.Demo;

/// <summary>
/// 服务配置
/// </summary>
internal static class Services
{
    /// <summary>
    /// 添加服务到容器
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    internal static IServiceCollection AddServices(this IServiceCollection service)
    {
        service.AddAutoDi(); // 自动注册到DI容器

        service.AddCorsPolicy(); // 跨域

        service.AddControllers(opt =>
        {
            opt.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "未从Request body中获取到请求内容");
        });

        service.AddSwagger(); // swagger

        service.AddModelCheck(); // 自定义模型验证返回内容

        service.AddSoapCore(); // webservice

        service.AddTimedTask(); // 定时任务

        return service;
    }
}