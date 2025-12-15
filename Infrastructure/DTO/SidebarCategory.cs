using System.Collections.ObjectModel;
using Infrastructure.EF.Enum;

namespace Infrastructure.DTO
{
    public class SidebarCategory : BindableBase
    {
        private MediaType _type;
        public MediaType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private string _label;
        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        private ObservableCollection<MediaEditModel> _allItems = new();
        public ObservableCollection<MediaEditModel> AllItems
        {
            get => _allItems;
            set => SetProperty(ref _allItems, value);
        }

        public ObservableCollection<MediaEditModel> TopItems { get; } = new();


        public int Count => AllItems.Count;
        public int RemainingCount => System.Math.Max(0, Count - 5);
        public bool HasMore => RemainingCount > 0;


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public void AddItem(MediaEditModel item)
        {
            AllItems.Add(item); // Добавляем в конец (или Insert(0, item) для начала)

            UpdateVisuals();
        }

        public void RemoveItem(Guid id)
        {
            var item = AllItems.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                AllItems.Remove(item);
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            // Обновляем TopItems
            TopItems.Clear();
            foreach (var item in AllItems.Take(5))
            {
                TopItems.Add(item);
            }

            // Уведомляем UI об изменении счетчиков
            RaisePropertyChanged(nameof(Count));
            RaisePropertyChanged(nameof(RemainingCount));
            RaisePropertyChanged(nameof(HasMore));
        }
    }
}
