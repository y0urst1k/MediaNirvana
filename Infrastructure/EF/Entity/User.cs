namespace Infrastructure.EF.Entity
{
    public class User : Base.Entity
    {
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string? SettingsJson { get; set; }

        public List<UserInteraction> Interactions { get; set; } = new();
        public List<UserLabel> Labels { get; set; } = new();
    }
}