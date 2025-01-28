using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM2AExtension.Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
namespace EM2AExtension.Extensions
{
    public class Em2AExtension : Extension
    {
        public override ExtensionConfiguration ExtensionConfiguration => throw new NotImplementedException();
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            base.InitializeServices(serviceCollection);
            serviceCollection.AddSingleton<Maker>();
        }
    }
}
