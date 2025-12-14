using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dialogs.Views
{
    public partial class EditContentDialogView : UserControl
    {
        public EditContentDialogView()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}