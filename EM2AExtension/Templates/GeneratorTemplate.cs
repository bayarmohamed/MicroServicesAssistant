using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM2AExtension.Templates
{
    public class Rootobject
    {
        public string runtime { get; set; }
        public Documentgenerator documentGenerator { get; set; }
        public Codegenerators codeGenerators { get; set; }
    }

    public class Documentgenerator
    {
        public Aspnetcoretoopenapi aspNetCoreToOpenApi { get; set; }
    }

    public class Aspnetcoretoopenapi
    {
        public string project { get; set; }
        public bool noBuild { get; set; }
    }

    public class Codegenerators
    {
        public Openapitocsharpclient openApiToCSharpClient { get; set; }
    }

    public class Openapitocsharpclient
    {
        public string @namespace { get; set; }
        public string output { get; set; }
        public bool generateClientInterfaces { get; set; }
    }
}
