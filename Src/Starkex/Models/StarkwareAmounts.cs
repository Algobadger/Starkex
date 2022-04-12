using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public record StarkwareAmounts(
        BigInteger QuantumsAmountSynthetic,
        BigInteger QuantumsAmountCollateral,
        BigInteger AssetIdSynthetic,
        BigInteger AssetIdCollateral,
        bool IsBuyingSynthetic)
    {

    }
}
