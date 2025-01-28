using EM2AExtension.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace EM2AExtension
{
    public partial class Wizard : DialogWindow
    {
        public Wizard()
        {
            InitializeComponent();
            DataContext = new WizardViewModel();
        }
    }
}
