using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM2AExtension.Logic
{
    public class Maker
    {
        public string FindSolutionPath(string currentDirectory)
        {
            var directory = new DirectoryInfo(currentDirectory);
            var files = Directory.GetFiles(directory.FullName, "*.sln", SearchOption.AllDirectories);
            return files[0];            
        }
        public async Task AttachProjectToCurrentSolution(string projectName ,string projectPath)
        {
            try
            {
                string currentDir = Directory.GetCurrentDirectory();
                string solutionPath = FindSolutionPath(currentDir);


                //string solutionPath = $"{solutionName}"; // Existing solution path
                string newProjectName = $"{projectName}";
                string newProjectPath = $@"{projectPath}\{newProjectName}";

                // Initialize MSBuildWorkspace
                using var workspace = MSBuildWorkspace.Create();

                // Load the existing solution

                var solution = workspace.CurrentSolution; // .OpenSolutionAsync(solutionPath).GetAwaiter().GetResult();
                var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
                //var WSworkspace = componentModel.GetService<Microsoft.VisualStudio.LanguageServices.VisualStudioWorkspace>();

              // var _roslynSolution = WSworkspace.CurrentSolution;

                // Create a new project programmatically
                var projectInfo = Microsoft.CodeAnalysis.ProjectInfo.Create(
                    id: Microsoft.CodeAnalysis.ProjectId.CreateNewId(),
                    version: Microsoft.CodeAnalysis.VersionStamp.Create(),
                    name: newProjectName,
                    assemblyName: newProjectName,
                    language: "C#",
                    filePath: newProjectPath);

                // Add the new project to the solution
                var updatedSolution = solution.AddProject(projectInfo);
                workspace.TryApplyChanges(updatedSolution);

                // Save the solution
                File.WriteAllText(solutionPath, updatedSolution.FilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<string> CreateProject(string projectName)
        {
            projectName = $"{projectName}.csproj";           

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectName}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Exe");
            propertyGroup.AddProperty("TargetFramework", "net6.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });

            // Save the .csproj file
            project.Save(csprojPath);
            return await Task.FromResult(csprojPath);
        }
        public async Task AddProjectToSolution(string project)
        {
            EnvDTE80.DTE2 dte = ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as EnvDTE80.DTE2;

            if (dte != null)
            {
                var helper = new SolutionHelper(dte);

                // Specify the path to the project file you want to add
                helper.AddProjectToSolution(project);
            }
        }
    }
}
