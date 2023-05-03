using Net.WebApi.Demo.Common.OtherExts;
using Net.WebApi.Demo.Common.SwaggerExts;
using Net.WebApi.Demo.Common.WebServiceExts;
using System.Diagnostics;

namespace Net.WebApi.Demo;

/// <summary>
/// http 请求管道配置
/// </summary>
internal static class Pipeline
{
    /// <summary>
    /// 添加请求管道
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    internal static WebApplication AddPipeline(this WebApplication app)
    {
        app.Use404ExCapture(); // 404异常捕获

        app.UseGlobalExCapture(); // 全局异常捕获

        if (!Debugger.IsAttached)
        {
            app.UseHttpsRedirection(); // 强制https
        }

        app.UseCors(); // 跨域

        app.UseSwaggerAndUi(); // swagger

        app.UseCustomMiddleware(); // 自定义中间件

        app.UseWebService(); // webservice

        app.MapControllers();

        app.Run();

        return app;
    }
}