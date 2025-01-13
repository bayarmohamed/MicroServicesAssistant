using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM2AExtension.Models
{
    public class GeneratedProjectModel
    {
        public EnvDTE.Project CreatedProject { get; set; }
        public EnvDTE.Project CreatedSdkProject { get; set; }
        public EnvDTE.Project SDKGeneratorFolder { get; set; }        

    }
}
