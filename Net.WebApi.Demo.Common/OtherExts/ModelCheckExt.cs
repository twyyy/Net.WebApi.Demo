using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace Net.WebApi.Demo.Common.OtherExts;

/// <summary>
/// 模型验证扩展
/// </summary>
public static class ModelCheckExt
{
    /// <summary>
    /// 自定义模型验证返回内容
    /// </summary>
    /// <param name="service"></param>
    public static void AddModelCheck(this IServiceCollection service)
    {
        service.Configure<ApiBehaviorOptions>(opt =>
        {
            // 模型验证返回
            opt.InvalidModelStateResponseFactory = ac =>
            {
                // 获取错误消息
                var errors = ac.ModelState.AsParallel().Select(ms => new
                {
                    ms.Key,
                    Errors = ms.Value?.Errors.Select(e => e.ErrorMessage).ToList() ?? new List<string>()
                }).ToList();

                // 错误消息列表转换为json
                var errorsJson = JsonConvert.SerializeObject(errors);

                // 日志记录或输出
                if (Debugger.IsAttached)
                {
                    Console.WriteLine(errorsJson);
                }
                else
                {
                    var path = ac.HttpContext.Request.Path;
                    Log.Error("模型验证日志", $"请求地址: {path}\r\n异常信息: {errorsJson}");
                }

                // 返回错误消息
                return new JsonResult(new
                {
                    HttpStatusCode = 400,
                    Succeed = false,
                    Message = "请求失败",
                    Data = errors
                }, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            };
        });
    }
}