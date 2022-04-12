using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public class OrderWithNonceAndPrice : OrderWithNonce
    {
        public double HumanPrice { get; }

        public OrderWithNonceAndPrice(Order order, BigInteger nonce, double humanPrice) : base(order, nonce)
        {
            HumanPrice = humanPrice;
        }

        protected override MidpointRounding GetRoundingMode()
        {
            return Order.Side == StarkwareOrderSide.BUY ? MidpointRounding.ToPositiveInfinity : MidpointRounding.ToZero;
        }

        protected override double GetHumanAmount()
        {
            return HumanPrice * Order.HumanSize;
        }
    }
}
