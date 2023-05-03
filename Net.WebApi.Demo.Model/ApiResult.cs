using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.WebApi.Demo.Common.AppSettingsExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Net.WebApi.Demo.Model;

/// <summary>
/// API接口返回对象
/// </summary>
public class ApiResult : IActionResult
{
    /// <summary>
    /// http状态码
    /// </summary>
    public int HttpStatusCode { get; init; } = 200;

    /// <summary>
    /// 请求是否成功
    /// </summary>
    public bool Succeed { get; init; } = true;

    /// <summary>
    /// 返回消息
    /// </summary>
    public string Message { get; init; } = "请求成功";

    /// <summary>
    /// 返回数据
    /// </summary>
    public object? Data { get; init; } = new();

    /// <summary>
    /// 返回执行方法
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task ExecuteResultAsync(ActionContext context)
    {
        // 返回对象
        var response = context.HttpContext.Response;

        // 自定义返回头信息
        response.ContentType = "application/json; charset=utf-8";
        response.Headers["Date"] = DateTime.Now.ToString(AppSettings.CommonlyUsed.DateTimeFormat);
        response.StatusCode = HttpStatusCode;

        // 返回内容
        var respContent = JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            // 小驼峰命名
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        return response.WriteAsync(respContent);
    }
}