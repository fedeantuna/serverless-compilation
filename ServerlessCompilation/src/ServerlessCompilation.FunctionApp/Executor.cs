using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;
using ServerlessCompilation.FunctionApp.Models;

namespace ServerlessCompilation.FunctionApp
{
    public static class Executor
    {
        private const String DefaultNamespace = "ServerlessRuntimeCompilation";
        private const String DefaultClass = "TestClass";
        private const String DefaultMethod = "GetMessage";
        private const String DefaultCode = @"
        using System;
        namespace ServerlessRuntimeCompilation
        {
            public class TestClass
            {
                public string GetMessage()
                {
                    return $""Hello World!"";
                }
            }
        }";

        [FunctionName("Execute")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Request received.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var codeDefinition = JsonConvert.DeserializeObject<CodeDefinition>(requestBody);
            var defNamespace = codeDefinition?.Namespace ?? DefaultNamespace;
            var defClass = codeDefinition?.Class ?? DefaultClass;
            var defMethod = codeDefinition?.Method ?? DefaultMethod;
            var defParameters = codeDefinition?.Parameters;
            var code = codeDefinition?.CSharpCode ?? DefaultCode;

            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var assemblyName = Path.GetRandomFileName();
            var references = new MetadataReference[] {
                MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location)
            };

            log.LogInformation("Added references.");

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var memoryStream = new MemoryStream();
            var compilationResult = compilation.Emit(memoryStream);

            if (compilationResult.Success)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                var assembly = AssemblyLoadContext.Default.LoadFromStream(memoryStream);

                var assemblyType = assembly.GetType($"{defNamespace}.{defClass}");
                var instance = assembly.CreateInstance($"{defNamespace}.{defClass}");
                var method = assemblyType.GetMember(defMethod).Single() as MethodInfo;

                var result = method.Invoke(instance, defParameters);

                return new OkObjectResult(JsonConvert.SerializeObject(result));
            }
            else
            {
                var failures = compilationResult.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);

                return new OkObjectResult(JsonConvert.SerializeObject(failures));
            }
        }
    }
}
