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
        public string CreateProject(string projectName)
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
            propertyGroup.AddProperty("TargetFramework", "net9.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });

            // Save the .csproj file
            project.Save(csprojPath);
            return csprojPath;
        }
        public Tuple<string,ProjectRootElement> CreateApiProject(string folder, string projectName)
        {
            var projectFolder = projectName;
            projectName = $"{projectName}.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory + $"\\{folder}\\" , $"{projectFolder}");
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
            itemGroup.AddItem("PackageReference", "NSwag.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath,project);
        }
        public Tuple<string, ProjectRootElement> CreateApiProjectInSelectedFolder(string projectName, string selectedFolder)
        {
            var originalPrjName = projectName;
            Solution2 solution = (Solution2)dte.Solution;
            SelectedItem selectedItem = dte.SelectedItems.Item(1);

            var projectFolder = selectedFolder + @"\" + projectName + @"\" + projectName + ".Host";
            projectName = $"{projectName}.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectFolder}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk.Web";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Exe");
            propertyGroup.AddProperty("TargetFramework", "net9.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            itemGroup.AddItem("PackageReference", "Swashbuckle.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "6.6.2") });
            itemGroup.AddItem("PackageReference", "NSwag.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            itemGroup.AddItem("PackageReference", "NSwag.MSBuild", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            //Add MSBUILD Nswag Target
            var newTarget = project.CreateTargetElement("NSwag");

            newTarget.AfterTargets = "PostBuildEvent";

            // Create the Exec task for the target
            var execTaskInterface = project.CreateTaskElement("Exec");
            execTaskInterface.SetParameter("EnvironmentVariables",
               @"ASPNETCORE_ENVIRONMENT=Development;NSwag=true");
            execTaskInterface.SetParameter("Command",
                @"$(NSwagExe_Net90) run ..\Sdks\" + $"{originalPrjName}" +@".sdk\Generator\interface.nswag /variables:Configuration=$(Configuration)");            
            // Append the Exec task to the Target
            project.AppendChild(newTarget);
            
            newTarget.AppendChild(execTaskInterface);

            var execTaskFacade = project.CreateTaskElement("Exec");
            execTaskFacade.SetParameter("EnvironmentVariables",
               @"ASPNETCORE_ENVIRONMENT=Development;NSwag=true");
            execTaskFacade.SetParameter("Command",
                @"$(NSwagExe_Net90) run ..\Sdks\" + $"{originalPrjName}" + @".sdk\Generator\facade.nswag /variables:Configuration=$(Configuration)");
            // Append the Exec task to the Target
            
            newTarget.AppendChild(execTaskFacade);

            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath, project);
        }
        public Tuple<string, ProjectRootElement> CreateSDKLibraryProjectInSelectedFolder(string projectName, string selectedFolder)
        {
            Solution2 solution = (Solution2)dte.Solution;
            SelectedItem selectedItem = dte.SelectedItems.Item(1);

            var projectFolder = selectedFolder + @"\" + projectName + @"\Sdks\" + projectName + ".SDK";
            projectName = $"{projectName}.SDK.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectFolder}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("TargetFramework", "netstandard2.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath, project);
        }
        public Tuple<string, ProjectRootElement> CreateInterfaceProjectInSelectedFolder(string projectName, string selectedFolder)
        {
            var originalPrjName = projectName;
            Solution2 solution = (Solution2)dte.Solution;
            SelectedItem selectedItem = dte.SelectedItems.Item(1);

            var projectFolder = selectedFolder + @"\" + projectName + @"\" + projectName + ".Interface";
            projectName = $"{projectName}.Interface.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectFolder}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Library");
            propertyGroup.AddProperty("TargetFramework", "net9.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            itemGroup.AddItem("PackageReference", "Swashbuckle.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "6.6.2") });
            itemGroup.AddItem("PackageReference", "NSwag.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            itemGroup.AddItem("PackageReference", "NSwag.MSBuild", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
           
            
            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath, project);
        }
        public Tuple<string, ProjectRootElement> CreateFacadeProjectInSelectedFolder(string projectName, string selectedFolder)
        {
            var originalPrjName = projectName;
            Solution2 solution = (Solution2)dte.Solution;
            SelectedItem selectedItem = dte.SelectedItems.Item(1);

            var projectFolder = selectedFolder + @"\" + projectName + @"\" + projectName + ".Facade";
            projectName = $"{projectName}.Facade.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectFolder}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Library");
            propertyGroup.AddProperty("TargetFramework", "net9.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            itemGroup.AddItem("PackageReference", "Swashbuckle.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "6.6.2") });
            itemGroup.AddItem("PackageReference", "NSwag.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            itemGroup.AddItem("PackageReference", "NSwag.MSBuild", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });


            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath, project);
        }
        public Tuple<string, ProjectRootElement> CreateBLInSelectedFolder(string projectName, string selectedFolder)
        {
            var originalPrjName = projectName;
            Solution2 solution = (Solution2)dte.Solution;
            SelectedItem selectedItem = dte.SelectedItems.Item(1);

            var projectFolder = selectedFolder + @"\" + projectName + @"\" + projectName + ".BL";
            projectName = $"{projectName}.BL.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectFolder}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Library");
            propertyGroup.AddProperty("TargetFramework", "net9.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            itemGroup.AddItem("PackageReference", "Swashbuckle.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "6.6.2") });
            itemGroup.AddItem("PackageReference", "NSwag.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            itemGroup.AddItem("PackageReference", "NSwag.MSBuild", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });


            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath, project);
        }
        public Tuple<string, ProjectRootElement> CreateAbstractionBLInSelectedFolder(string projectName, string selectedFolder)
        {
            var originalPrjName = projectName;
            Solution2 solution = (Solution2)dte.Solution;
            SelectedItem selectedItem = dte.SelectedItems.Item(1);

            var projectFolder = selectedFolder + @"\" + projectName + @"\" + projectName + ".IBL";
            projectName = $"{projectName}.IBL.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectFolder}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Library");
            propertyGroup.AddProperty("TargetFramework", "net9.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            itemGroup.AddItem("PackageReference", "Swashbuckle.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "6.6.2") });
            itemGroup.AddItem("PackageReference", "NSwag.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            itemGroup.AddItem("PackageReference", "NSwag.MSBuild", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });


            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath, project);
        }
        public Tuple<string, ProjectRootElement> CreateDomainInSelectedFolder(string projectName, string selectedFolder)
        {
            var originalPrjName = projectName;
            Solution2 solution = (Solution2)dte.Solution;
            SelectedItem selectedItem = dte.SelectedItems.Item(1);

            var projectFolder = selectedFolder + @"\" + projectName + @"\" + projectName + ".Domain";
            projectName = $"{projectName}.Domain.csproj";

            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{projectFolder}");
            Directory.CreateDirectory(projectPath);
            var csprojPath = Path.Combine(projectPath, $"{projectName}");

            // Create a new project root
            ProjectRootElement project = ProjectRootElement.Create();

            // Set the project SDK
            project.Sdk = "Microsoft.NET.Sdk";

            // Add properties (e.g., target framework)
            var propertyGroup = project.AddPropertyGroup();
            propertyGroup.AddProperty("OutputType", "Library");
            propertyGroup.AddProperty("TargetFramework", "net9.0");

            // Add an example package reference
            var itemGroup = project.AddItemGroup();
            itemGroup.AddItem("PackageReference", "Newtonsoft.Json", new[] { new KeyValuePair<string, string>("Version", "13.0.3") });
            itemGroup.AddItem("PackageReference", "Swashbuckle.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "6.6.2") });
            itemGroup.AddItem("PackageReference", "NSwag.AspNetCore", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });
            itemGroup.AddItem("PackageReference", "NSwag.MSBuild", new[] { new KeyValuePair<string, string>("Version", "14.2.0") });


            // Save the .csproj file
            project.Save(csprojPath);
            return Tuple.Create(csprojPath, project);
        }

        public void AddLaunchSettings(Project project)
        {
            var projectPath = Path.Combine(Environment.CurrentDirectory, $"{project.Name}");
            string launchSettingsPath = Path.Combine(projectPath, "Properties", "launchSettings.json");
            dynamic launchSettings = new
            {
                profiles = new
                {
                    API = new
                    {
                        commandName = "Project",
                        launchBrowser = true,
                        launchUrl = "swagger",
                        applicationUrl = "https://localhost:5001",
                        environmentVariables = new
                        {
                            ASPNETCORE_ENVIRONMENT = "Development"
                        }
                    }
                }
            };
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(launchSettings, Newtonsoft.Json.Formatting.Indented);
            Directory.CreateDirectory(Path.Combine(projectPath, "Properties")); // Ensure Properties folder exists
            File.WriteAllText(launchSettingsPath, jsonContent);
            project.Save();
            
        }
        public void ModifyLaunchSettings(Project project,string applicationName, string property, string value)
        {
            try
            {
                // Get the path to launchSettings.json
                var projectPath = Path.Combine(Environment.CurrentDirectory, $"{project.Name}");
                string launchSettingsPath = Path.Combine(projectPath, "Properties", "launchSettings.json");
                // Read the launchSettings.json file
                string jsonText = File.ReadAllText(launchSettingsPath);
                JObject launchSettings = JObject.Parse(jsonText);
                // Modify the launchSettings
                JObject profiles = (JObject)launchSettings["profiles"];
                JObject appProfile = (JObject)profiles[$"{applicationName}"];
                appProfile[$"{property}"] = $"{value}";

                JObject httpsProfile = (JObject)profiles["https"];
                httpsProfile[$"{property}"] = $"{value}";
                //httpsProfile["applicationUrl"] = "https://localhost:5001;http://localhost:5000";
                //httpsProfile["applicationUrl"] = "https://localhost:5001;http://localhost:5000"; 
                //httpsProfile["environmentVariables"]["ASPNETCORE_ENVIRONMENT"] = "Production";
                // Save the changes
                File.WriteAllText(launchSettingsPath, launchSettings.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
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
        public void CloseProject(Project prj)
        {
            prj.Save();
            dte.Solution.Remove(prj);
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
            project.Save();
        }
        public void AddFileToFolderProject(Project project, string folder, string fileName, string content)
        {
            // Chemin du dossier du projet
            string projectFolder = Path.GetDirectoryName(project.FullName);

            string forlderPath = Path.Combine(projectFolder, $"{folder}");
            string filePath = System.IO.Path.Combine(forlderPath, fileName);

            // 1. Create the Controllers folder in the file system
            if (!Directory.Exists(forlderPath))
            {
                Directory.CreateDirectory(forlderPath);
            }

            // 2. Add the folder to the project (appears in Solution Explorer)
            ProjectItem folderItem = project.ProjectItems
                                            .Cast<ProjectItem>()
                                            .FirstOrDefault(item => item.Name.Equals(folder, StringComparison.OrdinalIgnoreCase));

            if (folderItem == null)
            {
                // Folder doesn't exist; create it
                folderItem = project.ProjectItems.AddFolder(folder);
                
            }
           // ProjectItem folderItem = project.ProjectItems.AddFolder($"{folder}");

            // Créer et écrire le contenu du fichier
            System.IO.File.WriteAllText(filePath, content);

            // Ajouter le fichier au projet
            project.ProjectItems.AddFromFile(filePath);
            project.Save();
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
