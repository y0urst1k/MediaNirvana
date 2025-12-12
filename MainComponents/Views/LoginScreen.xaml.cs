using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaTracker.Views
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

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptLogin();
            }
        }

        private void AttemptLogin()
        {
            string username = UserBox.Text.Trim();
            string password = PassBox.Password.Trim();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // В React вы вызывали onLogin(username)
                // В C# мы вызываем событие
                LoginSuccess?.Invoke(this, username);

                // Очищаем пароль для безопасности
                PassBox.Password = "";
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите никнейм и пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}