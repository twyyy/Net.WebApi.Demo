using Net.WebApi.Demo;
using Net.WebApi.Demo.Common.OtherExts;

var builder = WebApplication.CreateBuilder(args);

// ��ʼ��ϵͳ����
builder.Configuration.InitAppSettings();

// ��ӷ�������
builder.Services.AddServices();

// ��������ܵ�
var app = builder.Build();
app.AddPipeline();