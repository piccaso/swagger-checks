using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class HoaxlyTests
    {
        [Test]
        public async Task CanCreateClasses() {
            var schema = await JsonSchema4.FromFileAsync("hoaxly-inbox-schema.json");
            var generator = new CSharpGenerator(schema, new CSharpGeneratorSettings() {
                GenerateDataAnnotations = false,
                GenerateDefaultValues = false,
                GenerateJsonMethods = false,
                ClassStyle = CSharpClassStyle.Poco,
            });
            var file = generator.GenerateFile();
            File.WriteAllText("hoaxly-inbox-schema.cs", file, Encoding.UTF8);
        }
    }
}
