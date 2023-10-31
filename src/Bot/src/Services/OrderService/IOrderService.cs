using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {    
    public interface IOrderService {
        public void AddOrder(User user, Order order);
        
        public void AddOrder(ulong userId, Order order);

        public void RemoveOrder(User user, int index);

        public void RemoveOrder(ulong userId, int index);

        public List<Order> GetOrders(ulong userId);
        
        public List<Order> GetOrders(User user);

        public Dictionary<ulong, List<Order>> GetAllOrders();

        public bool IsOrderingLocked();
        public void LockOrdering();
        public void UnlockOrdering();

    }
}

