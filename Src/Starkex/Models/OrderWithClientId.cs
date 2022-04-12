namespace Starkex.Models
{
    public class OrderWithClientId
    {
        public Order Order { get; }
        public string ClientId { get; }

        public OrderWithClientId(Order order, string clientId)
        {
            Order = order;
            ClientId = clientId;
        }
    }
}
