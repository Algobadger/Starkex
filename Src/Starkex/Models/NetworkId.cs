using Org.BouncyCastle.Math;

namespace Starkex.Models
{
    public class NetworkId
    {
        public readonly static NetworkId MAINNET = new(1, "a0b86991c6218b36c1d19d4a2e9eb0ce3606eb48", new BigInteger("02893294412a4c8f915f75892b395ebbf6859ec246ec365c3b1f56f47c3a0a5d", 16));
        public readonly static NetworkId ROPSTEN = new(3, "8707a5bf4c2842d46b31a405ba41b858c0f876c4", new BigInteger("02c04d8b650f44092278a7cb1e1028c82025dff622db96c934b611b84cc8de5a", 16));

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
