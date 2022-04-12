namespace Starkex.Models
{
    public class OrderWithClientIdAndQuoteAmount : OrderWithClientId
    {
        public double HumanQuoteAmount { get; }

        public OrderWithClientIdAndQuoteAmount(Order order, string clientId, double humanQuoteAmount) : base(order, clientId)
        {
            HumanQuoteAmount = humanQuoteAmount;
        }
    }
}
