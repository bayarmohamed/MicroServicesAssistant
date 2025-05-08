namespace EM2AExtension
{
    [Command(PackageIds.AddMicroservice)]
    internal sealed class CreateMicroService : BaseCommand<CreateMicroService>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                new MicroserviceWizard().Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
