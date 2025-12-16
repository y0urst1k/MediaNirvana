using System.Windows;
using Dialogs;
using Infrastructure;
using Infrastructure.EF.Context;
using Infrastructure.Service;
using MainComponents;
using MainComponents.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaNirvana
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return  Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // === 1. НАСТРОЙКА ЛОГИРОВАНИЯ ===

            var loggerFactory = new LoggerFactory();

            // Регистрируем фабрику
            containerRegistry.RegisterInstance<ILoggerFactory>(loggerFactory);

            // Регистрируем Generic Logger (Магия Unity/Prism)
            // Это позволяет внедрять ILogger<MyService>
            containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

            var optionsBuilder = new DbContextOptionsBuilder<MediaNirvanaDbContext>();

            optionsBuilder.UseSqlite("Data Source=media_nirvana.db");

            // 2. Регистрируем настройки (ВАЖНО: указываем Generic тип <MediaNirvanaDbContext>)
            containerRegistry.RegisterInstance<DbContextOptions<MediaNirvanaDbContext>>(optionsBuilder.Options);

            // 3. Вызываем регистрацию из модулей вручную
            var infraModule = new InfrastructureModue();
            var dialogsModule = new DialogsModule();

            // Внутри infraModule.RegisterTypes у вас есть строка:
            // containerRegistry.RegisterSingleton<MediaNirvanaDbContext>();
            // Когда Prism дойдет до создания контекста, он возьмет опции, которые мы зарегистрировали выше.
            infraModule.RegisterTypes(containerRegistry);
            dialogsModule.RegisterTypes(containerRegistry);

            // Теперь контейнер знает про ISessionService, DbContext и прочее!
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MainComponentsModule>();
        }

        // Переносим логику инициализации (Seeding) сюда
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Так как InfrastructureModule больше не проходит стандартный цикл инициализации,
            // его метод OnInitialized не вызовется сам. Вызываем логику здесь.

            try
            {
                var seeder = Container.Resolve<DatabaseSeedingService>();
                // Запускаем Seeding (лучше через Task.Run, чтобы окно показалось сразу)
                System.Threading.Tasks.Task.Run(async () => await seeder.SeedAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Seeding Error: {ex}");
            }
        }
    }
}
