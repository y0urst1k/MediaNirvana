using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MediaTracker.Views
{
    public static readonly DependencyProperty ItemsProperty =
    DependencyProperty.Register("Items", typeof(System.Collections.IEnumerable), typeof(ContentList));

    public System.Collections.IEnumerable Items
    {
        get { return (System.Collections.IEnumerable)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    // === События для родителя (MainView) ===
    // Это аналог props.onDelete, props.onViewDetail
    public event ContentCard.CardActionHandler ItemDeleted;
        public event ContentCard.CardActionHandler ItemViewDetail;
        public event ContentCard.CardActionHandler ItemEdited;

        public ContentList()
        {
            InitializeComponent();
        }

        // Обработчики событий, прилетающих от ContentCard внутри DataTemplate
        // Sender в данном случае - это сама карточка ContentCard
        private void OnCardDelete(object sender, ContentItem item)
        {
            ItemDeleted?.Invoke(this, item);
        }

        private void OnCardDetail(object sender, ContentItem item)
        {
            ItemViewDetail?.Invoke(this, item);
        }

        private void OnCardEdit(object sender, ContentItem item)
        {
            ItemEdited?.Invoke(this, item);
        }
    }
}