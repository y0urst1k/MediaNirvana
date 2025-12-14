using Infrastructure.EF.Entity.IndependentEntity;

namespace Infrastructure.Interface
{
    public interface ISessionService
    {
        User? CurrentUser { get; }

        bool IsAuthenticated { get; }
        Task Login(string username, string password);
        Task Logout();
    }
}