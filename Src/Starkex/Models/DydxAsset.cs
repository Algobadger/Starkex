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

        public DydxAsset(string value, string assetId, string resolution)
        {
            Value = value;
            if (assetId.StartsWith("0x"))
            {
                assetId = assetId[2..];
            }
            AssetId = new BigInteger(assetId,16);
            int.TryParse(resolution, out int res);
            Resolution = (int)Math.Log10(res);
        }
    }
}
