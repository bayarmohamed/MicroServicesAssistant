﻿global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EM2AExtension.Helpers;
using System.Runtime.InteropServices;
using System.Threading;

namespace EM2AExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.EM2AExtensionString)]
    public sealed class EM2AExtensionPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
           // PathConstants.VisixProjectPath = this.InstallPath;
            await this.RegisterCommandsAsync();
            //await MyCommand.InitializeAsync(this);
        }

    }
}