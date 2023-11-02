using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {
    public class MemStoreOrderService : IOrderService {
        public MemStoreOrderService() {
            m_Orders = new Dictionary<ulong, List<Order>>();
            m_IsOrderingLocked = true;
        }

        public Task AddOrder(User user, Order order) {
            AddOrder(user.DiscordId, order);
            return Task.CompletedTask;
        }

        public Task<List<Order>> GetOrders(User user) {
            return GetOrders(user.DiscordId);
        }

        public Task RemoveOrder(User user, int index) {
            RemoveOrder(user.DiscordId, index);
            return Task.CompletedTask;
        }

        public Task AddOrder(ulong userId, Order order) {
            if(!m_Orders.ContainsKey(userId)) {
                m_Orders.Add(userId, new List<Order>());
            }

            m_Orders[userId].Add(order);

            return Task.CompletedTask;
        }

        public Task<List<Order>> GetOrders(ulong userId) {
            if(!m_Orders.ContainsKey(userId)) {
                return Task.FromResult(new List<Order>());
            }

            return Task.FromResult(m_Orders[userId]);
        }

        public Task RemoveOrder(ulong userId, int index) {
            if(!m_Orders.ContainsKey(userId)) {
                return Task.CompletedTask;
            }

            m_Orders[userId].RemoveAt(index);
            return Task.CompletedTask;
        }

        public Task<List<Order>> GetAllOrders() {
            List<Order> orders = new List<Order>();
            
            foreach(var oder in m_Orders.Values) {
                orders.AddRange(oder);
            }

            return Task.FromResult(orders);
        }

        public Task<bool> IsOrderingLocked() {
            return Task.FromResult(m_IsOrderingLocked);
        }

        public Task LockOrdering() {
            m_IsOrderingLocked = true;
            return Task.CompletedTask;
        }

        public Task UnlockOrdering() {
            m_IsOrderingLocked = false;
            return Task.CompletedTask;
        }

        public Task DeleteAllOrders() {
            m_Orders.Clear();
            return Task.CompletedTask;
        }

        private Dictionary<ulong, List<Order>> m_Orders;

        private bool m_IsOrderingLocked;
    }
}