using System.Windows;

namespace MediaTracker
{
    public partial class App : Application
    {
        // Здесь можно написать логику, которая сработает ДО открытия окна.
        // Например, проверка обновлений или чтение настроек из файла.

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Аналог console.log("App started");
        }
    }
}
