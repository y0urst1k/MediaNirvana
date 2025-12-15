using System.Windows.Controls;
using Infrastructure.Interface;
using MainComponents.Events;

namespace MainComponents.ViewModels
{
    public class LoginScreenViewModel : BindableBase
    {
        private readonly ISessionService _sessionService;
        private readonly IEventAggregator _eventAggregator;

        // Свойства
        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        // Команды
        private DelegateCommand<object> _loginCommand;
        public DelegateCommand<object> LoginCommand => _loginCommand;

        public LoginScreenViewModel(
            ISessionService sessionService,
            IEventAggregator eventAggregator)
        {
            _sessionService = sessionService;
            _eventAggregator = eventAggregator;

            _loginCommand = new DelegateCommand<object>(ExecuteLogin, CanExecuteLogin);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Username) || e.PropertyName == nameof(Password))
                    LoginCommand.RaiseCanExecuteChanged();
            };
        }

        private bool CanExecuteLogin(object parameter)
        {
            // Проверка на пустоту имени
            if (string.IsNullOrWhiteSpace(Username)) return false;

            // Проверка пароля из PasswordBox (parameter)
            if (parameter is PasswordBox passwordBox)
            {
                return !string.IsNullOrWhiteSpace(passwordBox.Password);
            }
            return false;
        }

        private async void ExecuteLogin(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            // Получаем пароль напрямую из контрола для отправки
            string password = passwordBox?.Password ?? "";

            try
            {
                await _sessionService.Login(Username, Password);

                if (_sessionService.IsAuthenticated)
                {
                    // Отправляем событие!
                    _eventAggregator.GetEvent<LoginSuccessEvent>().Publish(Username);

                    // Очищаем поля
                    Username = string.Empty;;
                    passwordBox?.Clear();
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        "Неверный логин или пароль.",
                        "Ошибка входа",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Ошибка входа: {ex.Message}",
                    "Ошибка",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }
    }
}