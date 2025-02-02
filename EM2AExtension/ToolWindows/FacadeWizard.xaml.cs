using EM2AExtension.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace EM2AExtension
{
    public partial class FacadeWizard : DialogWindow
    {
        public FacadeWizard()
        {
            InitializeComponent();
            DataContext = new WizardViewModel();
        }
    }
}
