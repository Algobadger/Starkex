namespace Starkex.Models
{
    public class OrderWithClientIdWithPrice : OrderWithClientId
    {
        public double HumanPrice { get; }

        public OrderWithClientIdWithPrice(Order order, string clientId, double humanPrice) : base(order, clientId)
        {
            HumanPrice = humanPrice;
        }

    }
}
