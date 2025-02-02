namespace EM2AExtension
{
    [Command(PackageIds.MyContextMenuCommand)]
    internal sealed class CreateMicroService : BaseCommand<CreateMicroService>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("CreateMicroService", "Button clicked");
        }
    }
}
