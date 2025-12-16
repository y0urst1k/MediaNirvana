
using MainComponents.ViewModels;
using MainComponents.Views;

namespace MainComponents
{
    public class MainComponentsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // === РЕГИСТРАЦИЯ VIEW + VIEWMODEL ===
            // Prism автоматически связывает View с ViewModel по конвенции имен
            // (LoginScreen -> LoginScreenViewModel)

            // Основные экраны
            containerRegistry.RegisterForNavigation<LoginScreen, LoginScreenViewModel>();
            containerRegistry.RegisterForNavigation<MediaSidebar, MediaSidebarViewModel>();
            containerRegistry.RegisterForNavigation<ContentTrackerView, ContentTrackerViewModel>();

            // Экраны списков и деталей
            containerRegistry.RegisterForNavigation<ListsScreen, ListScreenViewModel>();
            containerRegistry.RegisterForNavigation<MediaDetailView, MediaDetailViewModel>();

            // Дочерние компоненты (если у них есть свои VM, которые требуют инъекций)
            // StatsOverview и SearchFilters используют вложенные VM в ContentTrackerViewModel,
            // поэтому их регистрировать не обязательно, если они создаются через new() в родителе.
            // Но если вы хотите внедрять зависимости в SearchFiltersViewModel, то лучше зарегистрировать:
            // containerRegistry.Register<SearchFiltersViewModel>();
        }
    }
}