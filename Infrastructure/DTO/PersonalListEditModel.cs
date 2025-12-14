using System.Collections.ObjectModel;
using Infrastructure.EF.Enum;

namespace Infrastructure.DTO
{
    public class PersonalListEditModel : BindableBase
    {
        private Guid _id;
        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private LabelType _type;
        public LabelType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        // Коллекция медиа-объектов
        private ObservableCollection<MediaEditModel> _items = new();
        public ObservableCollection<MediaEditModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        // Список ID (для удобства работы с БД)
        public List<Guid> ItemIds => Items.Select(i => i.Id).ToList();

        public List<LabelType> AvailableTypes { get; } = new List<LabelType>
        {
            LabelType.List,
            LabelType.Collection,
            LabelType.Priority
        };
    }
}
