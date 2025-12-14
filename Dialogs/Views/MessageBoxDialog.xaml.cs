using System.Windows;
using System.Windows.Controls;
using Dialogs.ViewModels;
namespace Dialogs.Views
{
    /// <summary>
    /// Логика взаимодействия для MessageBoxDialog.xaml
    /// </summary>
    public partial class MessageBoxDialog : UserControl
    {
        public MessageBoxDialog()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MessageBoxDialogViewModel;
            if (vm == null) return;

            MessageBoxResult result = MessageBox.Show(
                vm.Message,
                vm.Title,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            // Безопасный вызов через Delegate.Equals
            if (!Delegate.Equals(vm.RequestClose, null))
            {
                vm.RequestClose.Invoke(
                    new DialogResult(
                        result == MessageBoxResult.OK
                            ? ButtonResult.OK
                            : ButtonResult.Cancel
                    )
                );
            }

            var parent = Parent as Panel;
            if (parent != null) parent.Children.Remove(this);
        }
    }
}
