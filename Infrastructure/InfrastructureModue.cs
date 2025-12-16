using Infrastructure.EF.Context;
using Infrastructure.EF.Entity.ConnectingEntity;
using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;
using Infrastructure.Interface;
using Infrastructure.Repository;
using Infrastructure.Service;
using Infrastructure.Service.LegacyService;

namespace Infrastructure
{
    public class InfrastructureModue : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var seeder = containerProvider.Resolve<DatabaseSeedingService>();

            // Запускаем заполнение БД. 
            // Внимание: OnInitialized выполняется синхронно при старте.
            // Чтобы не блокировать UI (Splash Screen), лучше запустить это в фоне, 
            // но EF Core первый запрос все равно будет долгим (создание модели).
            // Для Seed данных синхронный вызов допустим, если данных немного.

            try
            {
                // Используем .Wait() или .GetAwaiter().GetResult(), так как интерфейс IModule синхронный.
                // Либо Task.Run(() => seeder.SeedAsync()).Wait();
                seeder.SeedAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не роняем приложение, если Seed не критичен
                System.Diagnostics.Debug.WriteLine($"DB Seed Failed: {ex}");
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MediaNirvanaDbContext>();
            containerRegistry.Register<DatabaseSeedingService>();

            // 2. Репозитории (Generic)
            containerRegistry.RegisterSingleton<IRepository<User>, Repository<User>>();
            containerRegistry.RegisterSingleton<IRepository<MediaItem>, Repository<MediaItem>>();
            containerRegistry.RegisterSingleton<IRepository<MediaInstance>, Repository<MediaInstance>>();
            containerRegistry.RegisterSingleton<IRepository<UserInteraction>, Repository<UserInteraction>>();
            containerRegistry.RegisterSingleton<IRepository<Relationship>, Repository<Relationship>>();
            containerRegistry.RegisterSingleton<IRepository<Tag>, Repository<Tag>>();

            // ДОБАВЛЕНО: Репозитории для списков
            containerRegistry.RegisterSingleton<IRepository<UserLabel>, Repository<UserLabel>>();
            containerRegistry.RegisterSingleton<IRepository<UserLabelLink>, Repository<UserLabelLink>>();

            // 3. Доменные сервисы (Специфичные)
            containerRegistry.RegisterSingleton<IService<MediaItem>, MediaItemService>();
            containerRegistry.RegisterSingleton<IService<UserInteraction>, UserInteractionService>();
            containerRegistry.RegisterSingleton<IService<UserLabel>, UserLabelService>();

            // 4. Доменные сервисы (Базовые)
            containerRegistry.RegisterSingleton<IService<User>, Service<User>>();
            containerRegistry.RegisterSingleton<IService<MediaInstance>, Service<MediaInstance>>();
            containerRegistry.RegisterSingleton<IService<Relationship>, Service<Relationship>>();
            containerRegistry.RegisterSingleton<IService<Tag>, Service<Tag>>();

            // ДОБАВЛЕНО: Сервис для связей списков
            containerRegistry.RegisterSingleton<IService<UserLabelLink>, Service<UserLabelLink>>();

            // 5. Фасадные сервисы (DTO Logic)
            containerRegistry.RegisterSingleton<ISessionService, SessionService>();
            containerRegistry.RegisterSingleton<TagService>();
            containerRegistry.RegisterSingleton<MediaEditService>();
            containerRegistry.RegisterSingleton<PersonalListEditModelService>(); // <-- Не забудьте это!

            // 6. Диалоги
            containerRegistry.Register<IEditContentDialogService, EditContentDialogService>();
            containerRegistry.Register<IUserDialog, UserDialog>();
        }
    }
}