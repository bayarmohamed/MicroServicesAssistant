using EM2AExtension.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace EM2AExtension
{
    public partial class MicroserviceWizard : DialogWindow
    {
        public MicroserviceWizard()
        {
            InitializeComponent();
            DataContext = new WizardViewModel();
        }
    }
}
