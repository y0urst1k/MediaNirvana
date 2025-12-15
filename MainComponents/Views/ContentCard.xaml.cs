using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MainComponents.Views
{
    public partial class ContentCard : UserControl
    {
        public ContentCard()
        {
            InitializeComponent();
        }

        // Команды, которые мы пробросим из ContentTrackerViewModel
        public static readonly DependencyProperty ViewDetailCommandProperty =
            DependencyProperty.Register("ViewDetailCommand", typeof(ICommand), typeof(ContentCard));
        public ICommand ViewDetailCommand { get => (ICommand)GetValue(ViewDetailCommandProperty); set => SetValue(ViewDetailCommandProperty, value); }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(ContentCard));
        public ICommand EditCommand { get => (ICommand)GetValue(EditCommandProperty); set => SetValue(EditCommandProperty, value); }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(ContentCard));
        public ICommand DeleteCommand { get => (ICommand)GetValue(DeleteCommandProperty); set => SetValue(DeleteCommandProperty, value); }
    }
}