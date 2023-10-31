using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {
    public class MemStoreOrderService : IOrderService {
        public MemStoreOrderService() {
            m_Orders = new Dictionary<ulong, List<Order>>();
        }

        public void AddOrder(User user, Order order) {
            AddOrder(user.DiscordId, order);
        }

        public List<Order> GetOrders(User user) {
            return GetOrders(user.DiscordId);
        }

        public void RemoveOrder(User user, int index) {
            RemoveOrder(user.DiscordId, index);
        }

        public void AddOrder(ulong userId, Order order) {
            if(!m_Orders.ContainsKey(userId)) {
                m_Orders.Add(userId, new List<Order>());
            }

            m_Orders[userId].Add(order);
        }

        public List<Order> GetOrders(ulong userId) {
            if(!m_Orders.ContainsKey(userId)) {
                return new List<Order>();
            }

            return m_Orders[userId];
        }

        public void RemoveOrder(ulong userId, int index) {
            if(!m_Orders.ContainsKey(userId)) {
                return;
            }

            m_Orders[userId].RemoveAt(index);
        }

        public Dictionary<ulong, List<Order>> GetAllOrders() {
            return m_Orders;
        }

        private Dictionary<ulong, List<Order>> m_Orders;
    }
}