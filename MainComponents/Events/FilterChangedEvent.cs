using Infrastructure.DTO;

namespace MainComponents.Events
{
    public class FilterChangedEvent : PubSubEvent<FilterState> { }
}
