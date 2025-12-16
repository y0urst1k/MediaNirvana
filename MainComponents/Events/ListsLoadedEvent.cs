using System.Collections.ObjectModel;
using Infrastructure.DTO;

namespace MainComponents.Events
{
    public class ListsLoadedEvent : PubSubEvent<ObservableCollection<PersonalListEditModel>> { }
}
