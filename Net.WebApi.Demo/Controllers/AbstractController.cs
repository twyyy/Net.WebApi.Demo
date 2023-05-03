using Microsoft.AspNetCore.Mvc;
using Net.WebApi.Demo.Model;

namespace Net.WebApi.Demo.Controllers;

/// <summary>
/// 抽象控制器
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public abstract class AbstractController : ControllerBase
{
    /// <summary>
    /// 返回成功
    /// </summary>
    /// <param name="msg">返回消息</param>
    /// <param name="data">返回数据</param>
    /// <returns></returns>
    protected ApiResult Ok(string msg = "请求成功", object? data = null) => new()
    {
        HttpStatusCode = 200,
        Succeed = true,
        Message = msg,
        Data = data ?? new object()
    };

    /// <summary>
    /// 返回失败
    /// </summary>
    /// <param name="msg">返回消息</param>
    /// <param name="data">返回数据</param>
    /// <returns></returns>
    protected ApiResult Fail(string msg = "请求失败", object? data = null) => new()
    {
        HttpStatusCode = 400,
        Succeed = false,
        Message = msg,
        Data = data ?? new object()
    };

    /// <summary>
    /// 返回异常
    /// </summary>
    /// <param name="msg">返回消息</param>
    /// <param name="data">返回数据</param>
    /// <returns></returns>
    protected ApiResult Error(string msg = "请求异常", object? data = null) => new()
    {
        HttpStatusCode = 500,
        Succeed = false,
        Message = msg,
        Data = data ?? new object()
    };

    /// <summary>
    /// 自定义返回
    /// </summary>
    /// <param name="httpStatusCode">http状态码</param>
    /// <param name="succeed">是否成功</param>
    /// <param name="msg">返回消息</param>
    /// <param name="data">返回数据</param>
    /// <returns></returns>
    protected ApiResult Custom(int httpStatusCode, bool succeed, string msg, object? data = null) => new()
    {
        HttpStatusCode = httpStatusCode,
        Succeed = succeed,
        Message = msg,
        Data = data ?? new object()
    };
}