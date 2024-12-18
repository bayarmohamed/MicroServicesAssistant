using EM2AExtension.Logic;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EM2AExtension
{
    [Command(PackageIds.AddProject)]
    internal sealed class AddProject : BaseCommand<AddProject>
    {
       

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                string projectName;
               
                Maker maker = new Maker();
                var project = await maker.CreateProject("Myproj4");
                await maker.AddProjectToSolution(project);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       
    }
  
}
