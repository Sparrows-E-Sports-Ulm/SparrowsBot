using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {
    public class MemStoreUserService : IUserService {
        public MemStoreUserService() {
            m_Users = new Dictionary<ulong, User>();

            // Add data for testing 
            m_Users.Add(269452193826865153, new User {DiscordId = 269452193826865153, DiscordName="Lars", FirstName="Lars", LastName="Pfrenger", PayPal="lars@pp.com"});
        }

        public void Add(User user) {
            m_Users.Add(user.DiscordId, user);
        }

        public void Delete(ulong id) {
            m_Users.Remove(id);
        }

        public void Delete(User user) {
            Delete(user.DiscordId);
        }

        public User Get(ulong id) {
            return m_Users[id];
        }

        public bool Exists(User user) {
            return Exists(user.DiscordId);
        }

        public bool Exists(ulong id) {
            return m_Users.ContainsKey(id);
        }

        private Dictionary<ulong, User> m_Users;
    }
}