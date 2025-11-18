using Infrastructure.EF.Context;
using Infrastructure.Interface;
using Infrastructure.Service;

namespace Infrastructure
{
    public class InfrastructureModue : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MediaNirvanaDbContext>();
            containerRegistry.Register<IUserDialog, UserDialog>();
        }
    }
}