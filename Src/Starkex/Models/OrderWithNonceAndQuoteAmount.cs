using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public class OrderWithNonceAndQuoteAmount : OrderWithNonce
    {
        public double HumanQuoteAmount { get; }

        public OrderWithNonceAndQuoteAmount(Order order, BigInteger nonce, double humanQuoteAmount) : base(order, nonce)
        {
            HumanQuoteAmount = humanQuoteAmount;
        }

        protected override double GetHumanAmount()
        {
            return HumanQuoteAmount;
        }

        protected override MidpointRounding GetRoundingMode()
        {
            return MidpointRounding.ToZero;
        }
    }
}
