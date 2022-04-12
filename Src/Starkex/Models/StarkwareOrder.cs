using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public record StarkwareOrder(StarkwareAmounts StarkwareAmounts,
                             StarkwareOrderType OrderType, BigInteger QuantumsAmountFee,
                             BigInteger AssetIdFee, string PositionId, BigInteger Nonce,
                             long ExpirationEpochHours)
    {


    }
}
