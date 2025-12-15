using System.Collections.ObjectModel;
using Infrastructure.DTO;

namespace MainComponents.Events
{
    public class MediaLibraryLoadedEvent : PubSubEvent<ObservableCollection<MediaEditModel>> { }
}
