using EM2AExtension.Helpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.ProjectSystem.VS;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VSLangProj;
using Project = EnvDTE.Project;
namespace EM2AExtension.Logic
{
    public class FoldersAndDirectoriesMaker
    {
        EnvDTE80.DTE2 dte;
        public FoldersAndDirectoriesMaker()
        {
            dte = ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as EnvDTE80.DTE2;
        }
        public Project AddProjectToSubSolutionFolder(string parentFolderName, string subFolderName, string projectFilePath)
        {
            Solution2 solution = (Solution2)dte.Solution;

            // Step 1: Find or Create Parent Solution Folder
            Project parentFolder = null;
            foreach (Project project in solution.Projects)
            {
                if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder && project.Name == parentFolderName)
                {
                    parentFolder = project;
                    break;
                }
            }

            if (parentFolder == null)
            {
                parentFolder = solution.AddSolutionFolder(parentFolderName);
            }

            // Step 2: Find or Create Sub Solution Folder within Parent
            EnvDTE80.SolutionFolder parentSolutionFolder = (EnvDTE80.SolutionFolder)parentFolder.Object;
            Project subFolder = null;
            //foreach (Project project in parentSolutionFolder.Parent.ProjectItems)
            //{
            //    if (project.Name == subFolderName)
            //    {
            //        subFolder = project;
            //        break;
            //    }
            //}

            if (subFolder == null)
            {
                subFolder = parentSolutionFolder.AddSolutionFolder(subFolderName);
                var sdkFolder = (((EnvDTE80.SolutionFolder)subFolder.Object)).AddSolutionFolder("Sdks");
                var deploymentFolder = (((EnvDTE80.SolutionFolder)subFolder.Object)).AddSolutionFolder("Deployment");
            }

            // Step 3: Add the Project to the Sub-Folder
            EnvDTE80.SolutionFolder subSolutionFolder = (EnvDTE80.SolutionFolder)subFolder.Object;
            return subSolutionFolder.AddFromFile(projectFilePath);            
        }

    }
}
