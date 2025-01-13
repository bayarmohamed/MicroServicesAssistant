using EM2AExtension.Helpers;
using EM2AExtension.Models;
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
        public GeneratedProjectModel AddProjectToSubSolutionFolder(string parentFolderName, string subFolderName, string projectFilePath)
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

            Project sdkFolder = null;
            Project sdkGeneratorFolder = null;


            if (subFolder == null)
            {
                subFolder = parentSolutionFolder.AddSolutionFolder(subFolderName);
                sdkFolder = (((EnvDTE80.SolutionFolder)subFolder.Object)).AddSolutionFolder("Sdks");
                //sdkGeneratorFolder = (((EnvDTE80.SolutionFolder)sdkFolder.Object)).AddSolutionFolder("Generator");
                var deploymentFolder = (((EnvDTE80.SolutionFolder)subFolder.Object)).AddSolutionFolder("Deployment");
            }

            // Step 3: Add the Project to the Sub-Folder
            EnvDTE80.SolutionFolder subSolutionFolder = (EnvDTE80.SolutionFolder)subFolder.Object;
            return new GeneratedProjectModel
            {

                CreatedProject = subSolutionFolder.AddFromFile(projectFilePath),
                CreatedSdkProject = sdkFolder,
                SDKGeneratorFolder = sdkGeneratorFolder
            };
        }
        public Project AddSDKProjectToSubSolutionFolder(Project project, string projectFilePath)
        {
            var result = (((EnvDTE80.SolutionFolder)project.Object)).AddFromFile(projectFilePath);
            return result;
        }

    }
}
