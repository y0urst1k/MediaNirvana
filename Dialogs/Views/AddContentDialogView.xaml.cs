using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dialogs.Views
{
    public partial class AddContentDialogView : UserControl
    {

        public AddContentDialogView()
        {
            InitializeComponent();
        }

        private void Inventory_Checked(object sender, RoutedEventArgs e)
        {
            if (InventoryDetails == null) return;

            bool hasPhysical = PhysicalCopyCheck.IsChecked == true;
            bool hasDigital = DigitalCopyCheck.IsChecked == true;

            InventoryDetails.Visibility = (hasPhysical || hasDigital)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Разрешить ввод только цифр
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

