using Net.WebApi.Demo;
using Net.WebApi.Demo.Common.OtherExts;

var builder = WebApplication.CreateBuilder(args);

// 初始化系统配置
builder.Configuration.InitAppSettings();

// 添加服务到容器
builder.Services.AddServices();

// 配置请求管道
var app = builder.Build();
app.AddPipeline();