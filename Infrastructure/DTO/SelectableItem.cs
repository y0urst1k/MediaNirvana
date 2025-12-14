namespace Infrastructure.DTO
{
    public class SelectableItem : BindableBase
    {
        private MediaEditModel _item;
        public MediaEditModel Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
