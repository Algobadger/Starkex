using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public abstract class OrderWithNonce
    {
        public Order Order { get; }
        public BigInteger Nonce { get; }

        public OrderWithNonce(Order order, BigInteger nonce)
        {
            Order = order;
            Nonce = nonce;
        }


        public BigInteger ToQuantums()
        {
            return ToQuantums(GetHumanAmount(), DydxAsset.USDC, GetRoundingMode());
        }

        public BigInteger ToQuantums(double humanAmount, DydxAsset asset, MidpointRounding roundingMode)
        {
            //decimal dAmount =(decimal) humanAmount;
            //var qSize = 1.0/ Math.Pow(10, asset.Resolution);
            //var data = Math.Round(dAmount / (decimal) qSize, roundingMode);
            //return new BigInteger(data.ToString());
            var quantumSize = Math.Pow(10.0, asset.Resolution);
            var res = Math.Round(quantumSize * humanAmount, roundingMode);
            return new BigInteger(res.ToString());
        }

        protected abstract MidpointRounding GetRoundingMode();

        protected abstract double GetHumanAmount();
    }
}
