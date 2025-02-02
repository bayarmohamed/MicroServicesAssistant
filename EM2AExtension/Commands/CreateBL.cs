namespace EM2AExtension
{
    [Command(PackageIds.AddBL)]
    internal sealed class CreateBL : BaseCommand<CreateBL>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                new BLWizard().Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
