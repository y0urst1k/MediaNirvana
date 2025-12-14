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
        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand => _loginCommand;

        public LoginScreenViewModel(
            ISessionService sessionService,
            IEventAggregator eventAggregator)
        {
            _sessionService = sessionService;
            _eventAggregator = eventAggregator;

            _loginCommand = new DelegateCommand(ExecuteLogin, CanExecuteLogin);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Username) || e.PropertyName == nameof(Password))
                    LoginCommand.RaiseCanExecuteChanged();
            };
        }

        private bool CanExecuteLogin() =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private async void ExecuteLogin()
        {
            try
            {
                await _sessionService.Login(Username, Password);

                if (_sessionService.IsAuthenticated)
                {
                    // Отправляем событие!
                    _eventAggregator.GetEvent<LoginSuccessEvent>().Publish(Username);

                    // Очищаем поля
                    Username = string.Empty;;
                    Password = string.Empty; ;
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