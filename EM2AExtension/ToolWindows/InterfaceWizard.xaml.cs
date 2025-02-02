using EM2AExtension.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace EM2AExtension
{
    public partial class InterfaceWizard : DialogWindow
    {
        public InterfaceWizard()
        {
            InitializeComponent();
            DataContext = new WizardViewModel();
        }
    }
}
