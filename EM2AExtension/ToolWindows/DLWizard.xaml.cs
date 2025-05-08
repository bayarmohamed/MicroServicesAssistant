using EM2AExtension.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace EM2AExtension
{
    public partial class DLWizard : DialogWindow
    {
        public DLWizard()
        {
            InitializeComponent();
            DataContext = new WizardViewModel();
        }
    }
}
