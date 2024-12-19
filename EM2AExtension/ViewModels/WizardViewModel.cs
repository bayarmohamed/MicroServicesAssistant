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
namespace EM2AExtension.ViewModels
{
    public class WizardViewModel
    {
        public ICommand CmdAddFile { get; }

        Maker maker;
        public List<string> Projects { get; set; } = new List<string>();
        public WizardViewModel()
        {
            CmdAddFile = new RelayCommand(ExecuteCommand, CanExecuteCommand);
            maker = new Maker();
            GetProjectsNames();
        }

        private bool CanExecuteCommand(object obj)
        {
            return true;
        }

        private void ExecuteCommand(object obj)
        {
            maker.AddFileToProject(maker.GetDefaultStartupProjects(), "console1.cs", CodeTemplates.ConsoleTemplate());
        }

        private void GetProjectsNames()
        {
            var projects = maker.GetDefaultStartupProjects();
            Projects.Add(projects.Name);
           
        }
    }
}
