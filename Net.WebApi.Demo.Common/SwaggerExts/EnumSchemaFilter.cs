using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;

namespace Net.WebApi.Demo.Common.SwaggerExts;

/// <summary>
/// Swagger枚举注释
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// 应用
    /// </summary>
    /// <param name="model"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;

        var newDesc = $"{model.Description}<br/><br/>";
        foreach (var name in Enum.GetNames(context.Type))
        {
            var field = context.Type.GetField(name);

            var descAttr = field?
                .GetCustomAttributes(false)
                .FirstOrDefault(w => w.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute;

            var desc = descAttr?.Description ?? "";
            var value = (int)Enum.Parse(context.Type, name);

            newDesc += $"{value} = {(string.IsNullOrWhiteSpace(desc) ? name : desc)}<br/>";
        }

        model.Description = newDesc;
    }
}