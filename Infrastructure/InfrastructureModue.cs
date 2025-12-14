using Infrastructure.EF.Context;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.Interface;
using Infrastructure.Repository;
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
            containerRegistry.RegisterSingleton<MediaNirvanaDbContext>();

            containerRegistry.RegisterSingleton<IRepository<User>, Repository<User>>();
            containerRegistry.RegisterSingleton<IRepository<MediaItem>, Repository<MediaItem>>();
            containerRegistry.RegisterSingleton<IRepository<MediaInstance>, Repository<MediaInstance>>();
            containerRegistry.RegisterSingleton<IRepository<UserInteraction>, Repository<UserInteraction>>();
            containerRegistry.RegisterSingleton<IRepository<Relationship>, Repository<Relationship>>();
            containerRegistry.RegisterSingleton<IRepository<Tag>, Repository<Tag>>();

            containerRegistry.RegisterSingleton<IService<User>, Service<User>>();
            containerRegistry.RegisterSingleton<IService<MediaItem>, Service<MediaItem>>();
            containerRegistry.RegisterSingleton<IService<MediaInstance>, Service<MediaInstance>>();
            containerRegistry.RegisterSingleton<IService<UserInteraction>, Service<UserInteraction>>();
            containerRegistry.RegisterSingleton<IService<Relationship>, Service<Relationship>>();
            containerRegistry.RegisterSingleton<IService<Tag>, Service<Tag>>();

            containerRegistry.RegisterSingleton<ISessionService, SessionService>();
            containerRegistry.RegisterSingleton<TagService>();
            containerRegistry.RegisterSingleton<MediaEditService>();

            containerRegistry.Register<IEditContentDialogService, EditContentDialogService>();

            containerRegistry.Register<IUserDialog, UserDialog>();
        }
    }
}