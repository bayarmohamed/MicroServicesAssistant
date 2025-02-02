namespace EM2AExtension
{
    [Command(PackageIds.AddDL)]
    internal sealed class CreateDL : BaseCommand<CreateDL>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                new DLWizard().Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
