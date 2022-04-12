using Org.BouncyCastle.Math;

namespace Starkex.Hashing
{
    public interface IHashCalculator<T>
    {
        public BigInteger CalculateHash(T message);
    }

}
