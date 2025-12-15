using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MainComponents.Views
{
    public partial class ContentList : UserControl
    {
        public ContentList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register("Items", typeof(IEnumerable), typeof(ContentList));
        public IEnumerable Items { get => (IEnumerable)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        // Команды, которые мы прокинем дальше в ContentCard
        public static readonly DependencyProperty ViewDetailCommandProperty =
            DependencyProperty.Register("ViewDetailCommand", typeof(ICommand), typeof(ContentList));
        public ICommand ViewDetailCommand { get => (ICommand)GetValue(ViewDetailCommandProperty); set => SetValue(ViewDetailCommandProperty, value); }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(ContentList));
        public ICommand EditCommand { get => (ICommand)GetValue(EditCommandProperty); set => SetValue(EditCommandProperty, value); }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(ContentList));
        public ICommand DeleteCommand { get => (ICommand)GetValue(DeleteCommandProperty); set => SetValue(DeleteCommandProperty, value); }
    }
}