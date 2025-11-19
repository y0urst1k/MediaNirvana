using Infrastructure.EF.Entity.IndependentEntity.DependentEntity;

namespace Infrastructure.EF.Entity.IndependentEntity
{
    public class User : Base.Entity
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTimeOffset RegistrationDate { get; set; }
        public string? SettingsJson { get; set; }

        public List<UserInteraction> Interactions { get; set; } = new();
        public List<UserLabel> Labels { get; set; } = new();
    }
}