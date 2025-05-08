using EM2AExtension.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace EM2AExtension
{
    public partial class BLWizard : DialogWindow
    {
        public BLWizard()
        {
            InitializeComponent();
            DataContext = new WizardViewModel();
        }
    }
}
