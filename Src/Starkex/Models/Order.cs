namespace Starkex.Models
{
    public record Order(string PositionId,
        double HumanSize,
        double LimitFee, // Max fee fraction, e.g. 0.01 is a max 1% fee.
        DydxAsset Market,
        StarkwareOrderSide Side,
        DateTime ExpirationIsoTimestamp)
    {

    }
}
