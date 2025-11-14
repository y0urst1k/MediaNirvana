using System.Windows;
using Infrastructure.Interface;

namespace Infrastructure.Service
{
    public class UserDialog : IUserDialog
    {
        public bool ConfirmWarning(string Warning, string Caption) => MessageBox.Show(Warning, Caption,MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes;

        public void ShowError(string Error, string Caption) => MessageBox.Show(Error, Caption, MessageBoxButton.OK, MessageBoxImage.Error);

        public void ShowInfo(string Information, string Caption) => MessageBox.Show(Information, Caption, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}