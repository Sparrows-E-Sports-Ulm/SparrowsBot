using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {    
    public interface IOrderService {
        public Task AddOrder(User user, Order order);
        
        public Task AddOrder(ulong userId, Order order);

        public Task RemoveOrder(User user, int index);

        public Task RemoveOrder(ulong userId, int index);

        public Task<List<Order>> GetOrders(ulong userId);
        
        public Task<List<Order>> GetOrders(User user);
        public Task<List<Order>> GetAllOrders();

        public Task DeleteAllOrders();

        public Task<bool> IsOrderingLocked();
        public Task LockOrdering();
        public Task UnlockOrdering();

    }
}

