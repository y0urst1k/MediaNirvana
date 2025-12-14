using Infrastructure.EF.Entity.IndependentEntity;
using Infrastructure.Interface;

namespace Infrastructure.Service
{
    public class SessionService : ISessionService
    {
        private readonly IService<User> _userService;
        public SessionService(IService<User> userService) 
        { 
            _userService = userService;
        }

        public User? CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        public async Task Login(string username, string password)
        {
            var user = await GetCurrentUser(username, password);

            if (user != null)
            {
                CurrentUser = user;
            }
        }

        public async Task Logout()
        {
            CurrentUser = null;
        }

        private async Task<User> GetCurrentUser(string username, string password) 
        {

            var users = await _userService.GetItemsAsync();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            return user;
        }
    }
}