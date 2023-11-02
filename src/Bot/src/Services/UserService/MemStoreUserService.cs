using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {
    public class MemStoreUserService : IUserService {
        public MemStoreUserService() {
            m_Users = new Dictionary<ulong, User>();

            // Add data for testing 
            m_Users.Add(269452193826865153, new User {DiscordId = 269452193826865153, DiscordName="Lars", FirstName="Lars", LastName="Pfrenger", PayPal="lars@pp.com"});
        }

        public Task Add(User user) {
            m_Users.Add(user.DiscordId, user);
            return Task.CompletedTask;
        }

        public Task<bool> Delete(ulong id) {
            m_Users.Remove(id);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(User user) {
            Delete(user.DiscordId);
            return Task.FromResult(true);
        }

        public Task<User> Get(ulong id) {
            return Task.FromResult(m_Users[id]);
        }

        public Task<bool> Exists(User user) {
            return Exists(user.DiscordId);
        }

        public Task<bool> Exists(ulong id) {
            return Task.FromResult(m_Users.ContainsKey(id));
        }

        private Dictionary<ulong, User> m_Users;
    }
}