using System.Collections.Concurrent;
using System.Diagnostics;

public static class OrderRepository
{
    private static readonly IDictionary<int,Order> _orders = new ConcurrentDictionary<int,Order>();
    public static async Task Insert(Order order){
        if(order is null)
            throw new ArgumentNullException(nameof(order));
        
        _orders.Add(order.Id,order);
    }
    public static async Task<Order?> GetAsync(int id)
    {
        if(!_orders.ContainsKey(id))
            return null;
        
        return _orders[id];
    }
}