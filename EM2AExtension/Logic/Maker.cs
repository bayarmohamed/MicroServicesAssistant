using EnvDTE;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Project = EnvDTE.Project;

namespace EM2AExtension.Logic
{
    public class Maker
    {
        EnvDTE80.DTE2 dte;
        public Maker()
        {
             dte = ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as EnvDTE80.DTE2;
        }
        public string FindSolutionPath(string currentDirectory)
        {
            var directory = new DirectoryInfo(currentDirectory);
            var files = Directory.GetFiles(directory.FullName, "*.sln", SearchOption.AllDirectories);
            return files[0];            
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
            

            if (dte != null)
            {
                var helper = new SolutionHelper(dte);

                // Specify the path to the project file you want to add
                helper.AddProjectToSolution(project);
            }
        }
        public Project GetSelectedProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Array activeSolutionProjects = (Array)dte.ActiveSolutionProjects;
            if (activeSolutionProjects.Length > 0)
            {
                return (Project)activeSolutionProjects.GetValue(0);
            }
            return null;
        }

        public void AddFileToProject(Project project, string fileName, string content)
        {
            // Chemin du dossier du projet
            string projectFolder = Path.GetDirectoryName(project.FullName);
            string filePath = System.IO.Path.Combine(projectFolder, fileName);

            // Créer et écrire le contenu du fichier
            System.IO.File.WriteAllText(filePath, content);

            // Ajouter le fichier au projet
            project.ProjectItems.AddFromFile(filePath);
        }
    }
}
