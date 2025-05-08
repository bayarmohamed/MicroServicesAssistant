namespace EM2AExtension
{
    [Command(PackageIds.AddFacade)]
    internal sealed class CreateFacade : BaseCommand<CreateFacade>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                new FacadeWizard().Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
