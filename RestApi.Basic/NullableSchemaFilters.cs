using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
// ReSharper disable ClassNeverInstantiated.Local

namespace RestApi.Basic {
    // adapted for swashbuckle core from : https://github.com/Alegrowin/Swashbuckle.AutoRestExtensions
    // related : https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/599

    internal static class NullableSchemaFilters {

        public static void EnableSaneNullHandling(this SwaggerGenOptions options) {
            options.SchemaFilter<NullableSchemaFilter>();
            options.SchemaFilter<NonNullableAsRequiredSchemaFilter>();
        }

        /// <summary>
        /// adds the 'x-nullable' extension to schemas
        /// </summary>
        private class NullableSchemaFilter : ISchemaFilter {
            public void Apply(Schema schema, SchemaFilterContext context) {
                var type = context.SystemType.GetTypeInfo();
                var nullable = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

                schema.Extensions.Add("x-nullable", nullable);
            }
        }

        /// <summary>
        /// marks all properties as required which are not 'x-nullable'
        /// </summary>
        private class NonNullableAsRequiredSchemaFilter : ISchemaFilter {
            public void Apply(Schema schema, SchemaFilterContext context) {
                {
                    if (schema.Properties == null) return;

                    var nonNullableProperties = schema.Properties
                        .Where(x => x.Value.HasNullable() && !x.Value.IsNullable())
                        .Select(x => x.Key)
                        .ToList();

                    if (!nonNullableProperties.Any()) return;

                    if (schema.Required == null) schema.Required = new List<string>();
                    foreach (var property in nonNullableProperties)
                        if (!schema.Required.Contains(property))
                            schema.Required.Add(property);
                }
            }
        }

        private static bool HasNullable(this Schema schema) => schema.Extensions != null &&
                                                               schema.Extensions.ContainsKey("x-nullable");

        private static bool IsNullable(this Schema schema) => schema.Extensions != null &&
                                                              schema.Extensions.TryGetValue("x-nullable", out var value) &&
                                                              value is bool b &&
                                                              b;
    }
}