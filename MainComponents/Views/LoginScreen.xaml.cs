using System.Windows;
using System.Windows.Controls;
using MainComponents.ViewModels;

namespace MainComponents.Views
{
    public partial class LoginScreen : UserControl
    {
        // Событие, которое мы кидаем наверх при успешном входе
        // Передаем string (username)
        public event EventHandler<string> LoginSuccess;

        public LoginScreen()
        {
            InitializeComponent();
            // Фокус на поле ввода при загрузке
            Loaded += (s, e) => UserBox.Focus();
        }

        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginScreenViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}