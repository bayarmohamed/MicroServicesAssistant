using EM2AExtension.Logic;
using EM2AExtension.Models;
using EM2AExtension.Templates;
using EM2AExtension.WpfCommands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace EM2AExtension.ViewModels
{
    public class WizardViewModel:INotifyPropertyChanged
    {
        public ICommand CmdAddFile { get; }
        public ICommand CmdAddProject { get; }
        public ICommand CmdAddApiTemplateProject { get; }
        public string PrjName { get => prjName; set { prjName = value; OnPropertyChanged(); } }
        public string SelectedProject { get => selectedProject; set { selectedProject = value; OnPropertyChanged(); } }
        public string FileName { get => fileName; set { fileName = value; OnPropertyChanged(); } }
        public List<string> Projects { get; set; } = new List<string>();
        public WizardViewModel()
        {
            CmdAddFile = new RelayCommand(ExecuteCommand, CanExecuteCommand);
            CmdAddProject = new RelayCommand( (obj) =>  AddNewProjectCommand(obj), CanExecuteAddNewProjectCommand);
            CmdAddApiTemplateProject = new RelayCommand( (obj) =>  AddApiProjectFromTemplateCommand(obj), CanExecuteAddApiProjectFromTemplateCommand);
            maker = new Maker();
            directoriesMaker = new FoldersAndDirectoriesMaker();
            GetProjectsNames();
        }
        private bool CanExecuteAddApiProjectFromTemplateCommand(object obj)
        {
            return true;
        }
        private  void AddApiProjectFromTemplateCommand(object obj)
        {
            //maker.AddSubSubFolder("BE", "Core", "Services");
            //maker.AddSolutionFolder("BE");
            //maker.AddSolutionFolder("FE");            
            if (!string.IsNullOrEmpty(PrjName))
            {
                CreateApi();
                CreateSdk();
            }
        }
        private void CreateApi()
        {           
           
            var project = maker.CreateApiProjectInSelectedFolder(prjName,"BE");
            projectInFolder = directoriesMaker.AddProjectToSubSolutionFolder("BE", prjName, project.Item1);
            maker.AddFileToProject(projectInFolder.CreatedProject, $"program.cs", CodeTemplates.programCode);
            maker.AddFileToFolderProject(projectInFolder.CreatedProject, "Controllers", $"EnvironmentController.cs", CodeTemplates.controllerCode);           
        }
        private void CreateSdk()
        {

            var project = maker.CreateSDKLibraryProjectInSelectedFolder(prjName, "BE");
            var result = directoriesMaker.AddSDKProjectToSubSolutionFolder(projectInFolder.CreatedSdkProject, project.Item1);
            maker.AddFileToFolderProject(result, "Generator", $"interface.nswag", CodeTemplates.NswagJsonGenCode(prjName));
        }
        private bool CanExecuteAddNewProjectCommand(object obj)
        {
            return true;
        }
        private  void AddNewProjectCommand(object obj)
        {
            if (!string.IsNullOrEmpty(PrjName))
            {
                var project =  maker.CreateProject(prjName);
                maker.AddProjectToSolution(project);               
            }           
        }
        private bool CanExecuteCommand(object obj)
        {
            return true;
        }
        private void ExecuteCommand(object obj)
        {
            maker.AddFileToProject(maker.GetSelectedProject(), $"{FileName}.cs", CodeTemplates.ConsoleTemplate);
        }
        private void GetProjectsNames()
        {
            selectedProjectFolder = maker.GetSelectedProject();
            SelectedProject = selectedProjectFolder is null ? "No project" : selectedProjectFolder.Name;
           
        }

        Maker maker;
        FoldersAndDirectoriesMaker directoriesMaker;
        EnvDTE.Project selectedProjectFolder;
        private string prjName;
        private string selectedProject;
        private string fileName;
        private GeneratedProjectModel projectInFolder;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
