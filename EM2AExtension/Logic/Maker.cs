using EM2AExtension.Helpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using VSLangProj;
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



        public async Task<string> CreateApiProject(string projectName)
        {
            projectName = $"{projectName}.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectName}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk.Web";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Exe");
            propertyGroup.AddProperty("TargetFramework", "net8.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            itemGroup.AddItem("PackageReference", "Swashbuckle.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "6.6.2") });

            // Save the .csproj file
            project.Save(csprojPath);
            return await Task.FromResult(csprojPath);
        }



        public void AddProjectToSolution(string project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrEmpty(project) || !File.Exists(project))
            {
                throw new FileNotFoundException("Project file not found.", project);
            }
            //https://learn.microsoft.com/en-us/dotnet/api/envdte.dte?view=visualstudiosdk-2022
            var solution = dte.Solution;

            if (solution == null || !solution.IsOpen)
            {
                throw new InvalidOperationException("No solution is open.");
            }

            // Add the project to the solution
            solution.AddFromFile(project);
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
        public void CreateApWebApiProjectFromTemplate()
        {

            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string templatePath = Path.Combine(assemblyLocation, "ProjectTemplates", "APITemplate.zip");

            string solutionPath =Path.GetDirectoryName(dte.Solution.FullName);
            string projectName = "APITemplate";
            string projectPath = Path.Combine(solutionPath, projectName);

            dte.Solution.AddFromTemplate(templatePath, projectPath, projectName, false);
            dte.StatusBar.Text = "Custom ASP.NET Core Web API Project Created!";
        }

      
    }
}
