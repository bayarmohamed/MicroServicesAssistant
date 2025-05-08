using EM2AExtension.Helpers;
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
    public class WizardViewModel : INotifyPropertyChanged
    {
        public ICommand CmdAddFile { get; }
        public ICommand CmdAddMicroservice { get; }
        public ICommand CmdAddInterface { get; }
        public ICommand CmdAddFacade { get; }
        public ICommand CmdAddBL { get; }
        public ICommand CmdAddDL { get; }
        public string PrjName { get => prjName; set { prjName = value; OnPropertyChanged(); } }
        public string InterfaceName { get => interfaceName; set { interfaceName = value; OnPropertyChanged(); } }
        public string FacadeName { get => facadeName; set { facadeName = value; OnPropertyChanged(); } }
        public string BLName { get => bLName; set { bLName = value; OnPropertyChanged(); } }
        public string DLName { get => dLName; set { dLName = value; OnPropertyChanged(); } }
        public string SelectedProject { get => selectedProject; set { selectedProject = value; OnPropertyChanged(); } }
        public string FileName { get => fileName; set { fileName = value; OnPropertyChanged(); } }
        public List<string> Projects { get; set; } = new List<string>();
        public WizardViewModel()
        {
            CmdAddFile = new RelayCommand(ExecuteCommand, CanExecuteCommand);
            CmdAddMicroservice = new RelayCommand((obj) => AddNewMicroserviceCommand(obj), CanExecuteAddNewMicroserviceCommand);
            CmdAddInterface = new RelayCommand((obj) => AddInterfaceCommand(obj), CanExecuteAddInterfaceCommand);
            CmdAddFacade = new RelayCommand((obj) => AddFacadeCommand(obj), CanExecuteAddFacadeCommand);
            CmdAddBL = new RelayCommand((obj) => AddBLCommand(obj), CanExecuteBLCommand);
            CmdAddDL = new RelayCommand((obj) => AddDLCommand(obj), CanExecuteAddDLCommand);

            maker = new Maker();
            directoriesMaker = new FoldersAndDirectoriesMaker();
            GetProjectsNames();
        }

        private bool CanExecuteAddDLCommand(object obj)
        {
            return CanDisplay();
        }

        private void AddDLCommand(object obj)
        {
            CreateDomainLibrary();
        }

        private bool CanExecuteBLCommand(object obj)
        {
            return CanDisplay();
        }

        private void AddBLCommand(object obj)
        {
            CreateBLLibrary();
            CreateAbstractBLLibrary();
        }

        private bool CanExecuteAddFacadeCommand(object obj)
        {
            return CanDisplay();
        }

        private void AddFacadeCommand(object obj)
        {
            CreateFacadeLibrary();
        }

        private bool CanExecuteAddInterfaceCommand(object obj)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return CanDisplay();
        }
        private void AddInterfaceCommand(object obj)
        {
            CreateInterfaceLibrary();
        }
        private void CreateApi()
        {
            var project = maker.CreateApiProjectInSelectedFolder(prjName, "BE");
            projectInFolder = directoriesMaker.AddProjectToSubSolutionFolder("BE", prjName, project.Item1);
            maker.AddFileToProject(projectInFolder.CreatedProject, $"program.cs", CodeTemplates.programCode);
            maker.AddFileToFolderProject(projectInFolder.CreatedProject, "Controllers", $"EnvironmentController.cs", CodeTemplates.controllerCode);
        }
        private void CreateFacadeSdk()
        {
            var project = maker.CreateSDKLibraryProjectInSelectedFolder($"{prjName}", "BE", "Facade");
            var result = directoriesMaker.AddSDKProjectToSubSolutionFolder(projectInFolder.CreatedSdkProject, project.Item1);
            maker.AddFileToFolderProject(result, "Generator", $"facade.nswag", CodeTemplates.NswagJsonGenCode(prjName, "Facade"));
        }
        private void CreateInterfaceSdk()
        {
            var project = maker.CreateSDKLibraryProjectInSelectedFolder($"{prjName}", "BE", "Interface");
            var result = directoriesMaker.AddSDKProjectToSubSolutionFolder(projectInFolder.CreatedSdkProject, project.Item1);
            maker.AddFileToFolderProject(result, "Generator", $"interface.nswag", CodeTemplates.NswagJsonGenCode(prjName, "Interface"));
        }
        private void CreateInterfaceLibrary()
        {
            if (!string.IsNullOrEmpty(InterfaceName))
            {
                var project = maker.CreateInterfaceProjectInSelectedFolder($"{InterfaceName}", "BE");
                var result = directoriesMaker.AddProjectToSelectedFolder(selectedProjectFolder, project.Item1);
                maker.AddFileToFolderProject(result, "Controller", $"MyInterfaceController.cs", CodeTemplates.controllerInterfaceCode);
            }
        }
        private void CreateFacadeLibrary()
        {
            if (!string.IsNullOrEmpty(FacadeName))
            {
                var project = maker.CreateFacadeProjectInSelectedFolder($"{FacadeName}", "BE");
                var result = directoriesMaker.AddProjectToSelectedFolder(selectedProjectFolder, project.Item1);
                maker.AddFileToFolderProject(result, "Controller", $"MyFacadeController.cs", CodeTemplates.controllerFacadeCode);
            }
        }
        private void CreateBLLibrary()
        {
            if (!string.IsNullOrEmpty(BLName))
            {
                var project = maker.CreateBLInSelectedFolder($"{BLName}", "BE");
                directoriesMaker.AddProjectToSelectedFolder(selectedProjectFolder, project.Item1);
            }
        }
        private void CreateAbstractBLLibrary()
        {
            if (!string.IsNullOrEmpty(BLName))
            {
                var project = maker.CreateAbstractionBLInSelectedFolder($"{BLName}", "BE");
                directoriesMaker.AddProjectToSelectedFolder(selectedProjectFolder, project.Item1);
            }
        }
        private void CreateDomainLibrary()
        {
            if (!string.IsNullOrEmpty(DLName))
            {
                var project = maker.CreateDomainInSelectedFolder($"{DLName}", "BE");
                var createdprj = directoriesMaker.AddProjectToSelectedFolder(selectedProjectFolder, project.Item1);
                maker.AddFileToFolderProject(createdprj, "DataBaseContext", $"{DLName}DBContext.cs", CodeTemplates.DbContext(DLName));
                maker.AddFileToFolderProject(createdprj, "DBFactory", $"{DLName}DBContextFactory.cs", CodeTemplates.DbContextFactory(DLName));
            }
        }
        private void SaveSolution()
        {
            maker.Save();
        }
        private bool CanExecuteAddNewMicroserviceCommand(object obj)
        {
            return true;
        }
        private void AddNewMicroserviceCommand(object obj)
        {
            if (!string.IsNullOrEmpty(PrjName))
            {
                CreateApi();
                CreateFacadeSdk();
                CreateInterfaceSdk();
                SaveSolution();
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
            SelectedProject = selectedProjectFolder is null ? "No project" : selectedProjectFolder?.Name;
        }
        private bool CanDisplay()
        {
            return selectedProjectFolder is not null &&
                   selectedProjectFolder.Kind == KnownProjectTypes.SolutionFolder &&
                  !ExcludedProjectNames.Contains(selectedProjectFolder.Name);
        }
        Maker maker;
        FoldersAndDirectoriesMaker directoriesMaker;
        EnvDTE.Project selectedProjectFolder;
        private string prjName;
        private string selectedProject;
        private string fileName;
        private GeneratedProjectModel projectInFolder;
        private string interfaceName;
        private string facadeName;
        private string bLName;
        private string dLName;
        private List<string> ExcludedProjectNames = new List<string> { "BE", "Sdks", "FE", "Deployment" };
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
