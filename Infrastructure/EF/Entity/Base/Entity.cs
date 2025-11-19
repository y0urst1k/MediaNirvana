using Infrastructure.Interface;

namespace Infrastructure.EF.Entity.Base
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; }
    }
}