using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
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
    }
}
