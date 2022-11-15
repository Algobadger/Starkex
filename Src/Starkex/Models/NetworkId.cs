using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public class NetworkId
    {
        public readonly static NetworkId MAINNET = new(1, "a0b86991c6218b36c1d19d4a2e9eb0ce3606eb48", new BigInteger("02893294412a4c8f915f75892b395ebbf6859ec246ec365c3b1f56f47c3a0a5d", 16));
        public readonly static NetworkId GOERLI = new(3, "F7a2fa2c2025fFe64427dd40Dc190d47ecC8B36e", new BigInteger("03bda2b4764039f2df44a00a9cf1d1569a83f95406a983ce4beb95791c376008", 16));

        public int Id { get; }
        public string AssetAddress { get; }
        public BigInteger CollateralAddressId { get; }

        internal NetworkId(int id, string assetAddress, BigInteger collateralAddressId)
        {
            Id = id;
            AssetAddress = assetAddress;
            CollateralAddressId = collateralAddressId;
        }
    }
}
