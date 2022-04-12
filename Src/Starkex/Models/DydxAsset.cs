using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public class DydxAsset
    {
        internal static DydxAsset USDC = new("USDC", new BigInteger("0", 16), 6);

        public string Value { get; }
        public BigInteger AssetId { get; }
        public int Resolution { get; }

        public DydxAsset(string value, BigInteger assetId, int resolution)
        {
            Value = value;
            AssetId = assetId;
            Resolution = resolution;
        }
    }
}
