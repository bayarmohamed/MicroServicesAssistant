using System.Text.Json;

namespace EM2AExtension.Templates
{
    public static class CodeTemplates
    {
        public static string ConsoleTemplate =
             @"using System;

namespace YourNamespace
{
    public class NewClass
    {
        public void HelloWorld()
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}";


        public static string programCode = @"

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = ""v1"";
    config.PostProcess = postProcess => { postProcess.Info.Title = ""API""; };
});

builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = ""facade"";
    config.ApiGroupNames = new[] { ""facade"" };
    config.PostProcess = postProcess => { postProcess.Info.Title = ""Facade contracts are used for inter-services communication""; };
});
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = ""interface"";
    config.ApiGroupNames = new[] { ""interface"" };
    config.PostProcess = postProcess => { postProcess.Info.Title = ""Interface contracts are used for external services communication""; };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(config =>
    {
        config.DocumentName = ""facade"";
        config.Path = ""/swagger/facade/swagger.json"";
    });

    app.UseOpenApi(config =>
    {
        config.DocumentName = ""interface"";
        config.Path = ""/swagger/interface/swagger.json"";
    });

    app.UseSwaggerUi(c =>
    {
        c.Path = ""/swagger/facade"";
        c.DocumentPath = ""/swagger/facade/swagger.json"";
    }).UseSwaggerUi(c =>
    {
        c.Path = ""/swagger/interface"";
        c.DocumentPath = ""/swagger/interface/swagger.json"";
    });
}

app.MapControllers();
app.Run();
";

        public static string controllerCode = @"
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route(""api/[controller]"")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(""Hello from ASP.NET Core Web API!"");
}
";
        public static string controllerInterfaceCode = @"
using Microsoft.AspNetCore.Mvc;

namespace Sales.Interface
{
    [ApiController]
    [Route(""interface/[controller]"")]
    [ApiExplorerSettings(GroupName =""interface"")]
    public class InterfaceController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(""Hello from ASP.NET Interface API!"");
    }

}
";
        public static string controllerFacadeCode = @"
using Microsoft.AspNetCore.Mvc;

namespace Sales.Facade
{
    [ApiController]
    [Route(""facade/[controller]"")]
    [ApiExplorerSettings(GroupName = ""facade"")]
    public class FacadeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(""Hello from ASP.NET Facade API!"");
    }
}
";
        public static string DbContextFactory(string DB) => @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using "+ DB +@".Domain.DataBaseContext;

namespace "+ DB + @".Domain.DBFactory;

 public class " + DB + @"ContextFactory : IDesignTimeDbContextFactory<" + DB + @"DBContext>
    {
        public " + DB + @"DBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<" + DB + @"DBContext>();
            optionsBuilder.UseSqlServer(""Server=.\\SQLEXPRESS;Database=" + DB + @";Trusted_Connection=true;Encrypt=false;"");

            return new " + DB + @"DBContext(optionsBuilder.Options);
        }
    }
";
        public static string DbContext(string DB) => @"
using Microsoft.EntityFrameworkCore;


namespace "+ DB +@".Domain.DataBaseContext
{
    public class " + DB +@"DBContext : DbContext
    {
        public "+ DB + @"DBContext(DbContextOptions<"+ DB + @"DBContext> options)
        : base(options) { }

       //DBSETS here

    }
}

";
        public static string NswagJsonGenCode(string prjName, string level)
        {
            var content = new Rootobject
            {
                runtime = "Net90",
                documentGenerator = new Documentgenerator
                {
                    aspNetCoreToOpenApi = new Aspnetcoretoopenapi
                    {
                        noBuild = true,
                        project = $"../../../{prjName}.Host/{prjName}.csproj"
                    }
                },
                codeGenerators = new Codegenerators
                {
                    openApiToCSharpClient = new Openapitocsharpclient
                    {
                        @namespace = $"{prjName}.{level}.Client",
                        generateClientInterfaces = true,
                        output = $"../{prjName}{level}SDKClient.cs"
                    }
                }
            };
            return System.Text.Json.JsonSerializer.Serialize(content,new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true});
        }
    }


   

}
