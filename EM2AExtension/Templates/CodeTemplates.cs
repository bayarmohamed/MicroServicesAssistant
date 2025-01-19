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
    config.Title = ""My API"";
    config.Version = ""v1"";
    config.Description = ""API documentation with NSwag"";
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();  // Swagger UI
    app.UseOpenApi();     // Serves the OpenAPI/Swagger document
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
