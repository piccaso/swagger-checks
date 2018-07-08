using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RestApi.Basic {
    public class NullableSchemaFilter : ISchemaFilter {
        public void Apply(Schema schema, SchemaFilterContext context) {
            var type = context.SystemType.GetTypeInfo();
            var nullable = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

            schema.Extensions.Add("x-nullable", nullable);
        }
    }

    public class NonNullableAsRequiredSchemaFilter : ISchemaFilter {
        public void Apply(Schema schema, SchemaFilterContext context) {
            {
                if (schema.Properties == null) return;
                
                var nonNullableProperties = schema.Properties
                    .Where(x => x.Value.HasNullable() && !x.Value.IsNullable())
                    .Select(x => x.Key)
                    .ToList();

                if (!nonNullableProperties.Any()) return;
                
                if (schema.Required == null) schema.Required = new List<string>();
                foreach (var property in nonNullableProperties) {
                    if (!schema.Required.Contains(property)) schema.Required.Add(property);
                }
            }
        }
    }

    public static class SchemaExtensions
    {
        public static bool HasNullable(this Schema schema) => schema.Extensions != null &&
                                                              schema.Extensions.ContainsKey("x-nullable");

        public static bool IsNullable(this Schema schema) => schema.Extensions != null &&
                                                             schema.Extensions.TryGetValue("x-nullable", out var value) &&
                                                             value is bool b &&
                                                             b;
    }

    
}
