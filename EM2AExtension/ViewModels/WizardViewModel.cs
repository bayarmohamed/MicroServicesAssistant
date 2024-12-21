using EM2AExtension.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem.VS;
using System.Windows.Input;
using EM2AExtension.WpfCommands;
using EM2AExtension.Templates;
using System.ComponentModel;
namespace EM2AExtension.ViewModels
{
    public class WizardViewModel:INotifyPropertyChanged
    {
        public ICommand CmdAddFile { get; }
        public ICommand CmdAddProject { get; }
        public string PrjName { get => prjName; set { prjName = value; OnPropertyChanged(); } }
        public string SelectedProject { get => selectedProject; set { selectedProject = value; OnPropertyChanged(); } }
        public string FileName { get => fileName; set { fileName = value; OnPropertyChanged(); } }
        public List<string> Projects { get; set; } = new List<string>();
        public WizardViewModel()
        {
            CmdAddFile = new RelayCommand(ExecuteCommand, CanExecuteCommand);
            CmdAddProject = new RelayCommand(async (obj) => await AddNewProjectCommand(obj), CanExecuteAddNewProjectCommand);
            maker = new Maker();
            GetProjectsNames();
        }

        private bool CanExecuteAddNewProjectCommand(object obj)
        {
            return true;
        }

        private async Task AddNewProjectCommand(object obj)
        {
            if (!string.IsNullOrEmpty(PrjName))
            {
                var project = await maker.CreateProject(prjName);
                await maker.AddProjectToSolution(project);
            }
           
        }

        private bool CanExecuteCommand(object obj)
        {
            return true;
        }

        private void ExecuteCommand(object obj)
        {
            maker.AddFileToProject(maker.GetSelectedProject(), $"{FileName}.cs", CodeTemplates.ConsoleTemplate());
        }

        private void GetProjectsNames()
        {
            var project = maker.GetSelectedProject();
            SelectedProject = project is null ? "No project" : project.Name;
           
        }

        Maker maker;
        private string prjName;
        private string selectedProject;
        private string fileName;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
