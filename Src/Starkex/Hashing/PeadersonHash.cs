using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Starkex.Hashing
{
    public class PedersonHash : IHashFunction
    {
        private static readonly BigInteger PRIME = new("800000000000011000000000000000000000000000000000000000000000001", 16);
        private static readonly Dictionary<BigInteger, Dictionary<BigInteger, BigInteger>> Cache = new();
        //private static final Map<BigInteger, Map<BigInteger, BigInteger>> CACHE = new ConcurrentHashMap<>();
        private readonly ECPoint _basePoint;

        public PedersonHash(ECPoint basePoint)
        {
            _basePoint = basePoint;
        }

        public BigInteger CreateHash(BigInteger left, BigInteger right)
        {
            ECPoint point = CalculateWith(_basePoint, 0, left);
            point = CalculateWith(point, 1, right);
            return point.XCoord.ToBigInteger();
        }

        private static ECPoint CalculateWith(ECPoint point, int index, BigInteger field)
        {
            ECPoint newPoint = point;
            CheckField(field);
            for (int i = 0; i < 252; i++)
            {
                ECPoint pt = ConstantPoints.GetECPoint(2 + index * 252 + i);

                if (point.XCoord.Equals(pt.XCoord))
                {
                    throw new Exception("Error computing pedersen hash");
                }
                if (!field.And(BigInteger.One).Equals(BigInteger.Zero))
                {
                    newPoint = newPoint.Add(pt);
                }
                field = field.ShiftRight(1);
            }
            return newPoint;
        }

        private static void CheckField(BigInteger field)
        {
            if (!(field.CompareTo(BigInteger.Zero) >= 0 && field.CompareTo(PRIME) < 0))
            {
                throw new Exception($"Input to pedersen hash out of range: {field}");
            }
        }

        public BigInteger HashFromCache(BigInteger left, BigInteger right)
        {
            if (Cache.TryGetValue(left, out var l2) == false)
            {
                l2 = new Dictionary<BigInteger, BigInteger>();
                Cache.TryAdd(left, l2);
            }
            if (l2.TryGetValue(right, out BigInteger? res))
            {
                return res;
            }
            else
            {
                res = CreateHash(left, right);
                l2.TryAdd(right, res);
                return res;
            }
        }
    }
}
