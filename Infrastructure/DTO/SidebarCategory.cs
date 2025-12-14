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

        private List<MediaEditModel> _allItems = new();
        public List<MediaEditModel> AllItems
        {
            get => _allItems;
            set => SetProperty(ref _allItems, value);
        }

        public List<MediaEditModel> TopItems => AllItems.Take(5).ToList();


        public int Count => AllItems.Count;
        public int RemainingCount => System.Math.Max(0, Count - 5);
        public bool HasMore => RemainingCount > 0;


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }
    }
}
