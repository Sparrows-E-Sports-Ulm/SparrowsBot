using System.Collections.Immutable;
using MongoDB.Bson;
using MongoDB.Driver;
using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {
    public class DBOrderService : IOrderService {
        private const string ORDERS_TABEL = "Orders";
        private const string ORDER_COUNTER_TABEL = "OrderCounter";

        public DBOrderService(MongoClient client) {
            m_DB = client.GetDatabase("Orders");
            m_Orders = m_DB.GetCollection<Order>(ORDERS_TABEL);
            m_OrderCounter = m_DB.GetCollection<OrderCounter>(ORDER_COUNTER_TABEL);
            m_IsOrderingLocked = true;
        }

        public async Task AddOrder(User user, Order order) {
            await AddOrder(user.DiscordId, order);
        }

        private async Task<int> UpdateOrderCount(ulong userId, int change) {
            var result = await m_OrderCounter.FindOneAndUpdateAsync<OrderCounter>(
                _ => _.DiscordId == userId, 
                Builders<OrderCounter>.Update.Inc(_ => _.OrderCount, change),
                new FindOneAndUpdateOptions<OrderCounter, OrderCounter> {IsUpsert = true, ReturnDocument = ReturnDocument.After}
            );

            return result.OrderCount;
        }

        public async Task AddOrder(ulong userId, Order order) {
            order.Index = await UpdateOrderCount(userId, 1);
            await m_Orders.InsertOneAsync(order);
        }

        public async Task DeleteAllOrders() {
            await m_DB.DropCollectionAsync(ORDERS_TABEL);
            await m_DB.DropCollectionAsync(ORDER_COUNTER_TABEL);
        }

        public async Task<List<Order>> GetAllOrders() {
            return await m_Orders.Find(Builders<Order>.Filter.Empty).SortBy(_ => _.DiscordUserId).ToListAsync();
        }

        public async Task<List<Order>> GetOrders(ulong userId) {
            return await m_Orders.Find(_ => _.DiscordUserId == userId).ToListAsync();
        }

        public async Task<List<Order>> GetOrders(User user) {
            return await GetOrders(user.DiscordId);
        }
        public async Task RemoveOrder(User user, int index) {
            await RemoveOrder(user.DiscordId, index);
        }

        public async Task<bool> RemoveOrder(ulong userId, int index) {
            var orders = await GetOrders(userId);

            index = index - 1;
            if(index < 0 || index > orders.Count - 1) {
                return false;
            }

            await m_Orders.DeleteOneAsync(_ => _.Id == orders[index].Id);

            return true;
        }

        public Task LockOrdering() {
            m_IsOrderingLocked = true;
            return Task.CompletedTask;
        }

        public Task UnlockOrdering() {
            m_IsOrderingLocked = false;
            return Task.CompletedTask;
        }

        public Task<bool> IsOrderingLocked() {
            return Task.FromResult(m_IsOrderingLocked);
        }

        private IMongoCollection<Order> m_Orders;
        private IMongoCollection<OrderCounter> m_OrderCounter;
        private IMongoDatabase m_DB;

        private bool m_IsOrderingLocked; 
    }
}