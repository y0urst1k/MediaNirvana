using System.Windows;
using Dialogs;
using Infrastructure;
using MainComponents;
using MainComponents.Views;

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
            // Здесь можно ничего не писать, если все регистрации вынесены в модули.
            // Но метод должен существовать.
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Если MainComponents - это отдельный проект/модуль:
            moduleCatalog.AddModule<InfrastructureModue>();
            moduleCatalog.AddModule<DialogsModule>();
            moduleCatalog.AddModule<MainComponentsModule>();
        }
    }
}
