using Org.BouncyCastle.Math;

namespace Starkex.Hashing
{
    public interface IHashFunction
    {
        /**
         * creates hash from the parameters
         *
         * @param left
         * @param right
         * @return hash number
         * @throws HashingException
         */
        BigInteger CreateHash(BigInteger left, BigInteger right);

        /**
         * first look up cache for the hash value if not exists there calculates hash and put into cache
         *
         *
         * @param left
         * @param right
         * @return hash number
         * @throws HashingException
         */
        BigInteger HashFromCache(BigInteger left, BigInteger right);
    }
}
