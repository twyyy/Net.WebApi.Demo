using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Net.WebApi.Demo.Common.OtherExts;
using System.Reflection;

namespace Net.WebApi.Demo.Common.SwaggerExts;

/// <summary>
/// API文档扩展
/// </summary>
public static class SwaggerExt
{
    /// <summary>
    /// 添加swagger服务到容器
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public static void AddSwagger(this IServiceCollection service)
    {
        service.AddSwaggerGen(opt =>
        {
            // 获取配置的项目名称
            var name = ConfigExt.Get<string>("Initial:Name") ?? "Asp.Net Demo";

            // 获取主程序集版本号
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0";

            // 配置文档的基础信息
            opt.SwaggerDoc(name, new OpenApiInfo
            {
                Version = version,
                Title = "接口文档"
            });
            opt.OrderActionsBy(o => o.RelativePath);

            // 获取配置的程序集
            var dlls = ConfigExt.Get<List<string>>("Initial:SwaggerDlls") ?? new List<string>();

            // 添加XML文件路径
            Parallel.ForEach(dlls, dll =>
            {
                // 拼接路径并检查文件是否存在
                var xmlPath = Path.Combine(AppContext.BaseDirectory, dll, ".xml");
                if (!File.Exists(xmlPath))
                {
                    return;
                }

                opt.IncludeXmlComments(xmlPath, true);
            });

            // 应用枚举注释
            opt.SchemaFilter<EnumSchemaFilter>();
        });
    }

    /// <summary>
    /// 请求管道中使用swagger
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static void UseSwaggerAndUi(this WebApplication app)
    {
        // 获取配置的项目名称
        var name = ConfigExt.Get<string>("Initial:Name") ?? "Asp.Net Demo";

        // 使用Swagger
        app.UseSwagger();

        // 使用SwaggerUI
        app.UseSwaggerUI(opt =>
        {
            opt.SwaggerEndpoint($"/swagger/{name}/swagger.json", "接口文档");
            opt.RoutePrefix = "swagger";
        });
    }
}