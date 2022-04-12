using Org.BouncyCastle.Math;
using Starkex.Converters;
using Starkex.Hashing;
using Starkex.Models;

namespace Starkex.Signatures
{

    public class StarkSigner
    {
        private static readonly StarkHashCalculator STARK_HASH_CALCULATOR = new StarkHashCalculator(new PedersonHash(ConstantPoints.GetECPoint(0)));
        private static readonly EcSigner EC_SIGNER = new EcSigner(StarkCurve.GetInstance());

        public Signature Sign(OrderWithClientId order, NetworkId networkId, BigInteger privateKey)
        {
            var starkOrder = StarkwareOrderConverter.FromOrderWithClientId(order, networkId);
            var hash = STARK_HASH_CALCULATOR.CalculateHash(starkOrder);
            return EC_SIGNER.Sign(privateKey, hash);
        }
    }
}
