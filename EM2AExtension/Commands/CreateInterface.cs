namespace EM2AExtension
{
    [Command(PackageIds.AddInterface)]
    internal sealed class CreateInterface : BaseCommand<CreateInterface>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                new InterfaceWizard().Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
