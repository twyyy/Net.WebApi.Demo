using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace Net.WebApi.Demo.Common.OtherExts;

/// <summary>
/// 异常处理扩展
/// </summary>
public static class ExceptionExt
{
    /// <summary>
    /// 返回头日期时间格式懒加载
    /// </summary>
    private static readonly Lazy<string> HeaderDateFormat = new(() =>
    {
        return ConfigExt.Get<string>("ReturnHeaderDate:Format") ?? "yyyy-MM-dd HH:mm:ss zzz";
    });

    /// <summary>
    /// 返回头日期时间时区偏移量懒加载
    /// </summary>
    private static readonly Lazy<int> HeaderDateTimezoneOffset = new(() =>
    {
        return ConfigExt.Get<int?>("ReturnHeaderDate:TimezoneOffset") ?? 8;
    });

    /// <summary>
    /// 使用404异常处理
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static void Use404ExCapture(this IApplicationBuilder app)
    {
        app.UseStatusCodePages(async context =>
        {
            // 返回对象
            var response = context.HttpContext.Response;

            // 返回头
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Date =
                new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromHours(HeaderDateTimezoneOffset.Value)).ToString(HeaderDateFormat.Value);
            response.StatusCode = 404;

            // 返回对象
            var result = new
            {
                HttpStatusCode = 404,
                Succeed = false,
                Message = "请求的资源不存在",
                Data = new { }
            };

            await response.WriteAsync(JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        });
    }

    /// <summary>
    /// 全局异常捕获
    /// </summary>
    /// <param name="app"></param>
    public static void UseGlobalExCapture(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder => builder.Run(async context =>
        {
            // 抓取全局异常并输出或记录
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            if (error != null && Debugger.IsAttached)
            {
                throw new Exception("全局异常", error);
            }
            else if (error != null)
            {
                Log.Error("全局异常", $"{error.Message} {error.StackTrace}");
            }

            // 返回对象
            var response = context.Response;

            // 返回头
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Date =
                new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromHours(HeaderDateTimezoneOffset.Value)).ToString(HeaderDateFormat.Value);
            response.StatusCode = 500;

            // 返回对象
            var result = new
            {
                HttpStatusCode = 500,
                Succeed = false,
                Message = "请求异常",
                Data = new { }
            };

            await response.WriteAsync(JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }));
    }
}