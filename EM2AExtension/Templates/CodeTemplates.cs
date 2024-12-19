using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM2AExtension.Templates
{
    public static class CodeTemplates
    {
        public static string ConsoleTemplate()
        {
            return @"using System;

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
        }
    }
}
