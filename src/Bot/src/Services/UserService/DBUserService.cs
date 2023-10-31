using MongoDB.Driver;
using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {
    class DBUserService : IUserService {
        public DBUserService(MongoClient client) {
            IMongoDatabase db = client.GetDatabase("Users");

            if(db.ListCollectionNames().ToList().Contains("Users")) {
                Console.WriteLine("Initializing Users Collection");
                db.CreateCollection("Users");
            }

            m_Collection = db.GetCollection<User>("Users");
        }

        public async Task Add(User user) {
            await m_Collection.InsertOneAsync(user);
        }

        public async Task<bool> Delete(ulong id) {
            DeleteResult result = await m_Collection.DeleteOneAsync(filter => filter.DiscordId == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> Delete(User user) {
            return await Delete(user.DiscordId);
        }

        public async Task<bool> Exists(User user) {
            return await Exists(user.DiscordId);
        }

        public async Task<bool> Exists(ulong id) {
            return await m_Collection.Find(_ => _.DiscordId == id).AnyAsync();
        }

        public async Task<User> Get(ulong id) {
            return await m_Collection.Find(_ => _.DiscordId == id).FirstAsync();
        }

        private IMongoCollection<User> m_Collection;
    }
}