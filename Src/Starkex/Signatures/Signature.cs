using Org.BouncyCastle.Math;

namespace Starkex.Signatures
{
    public record Signature(BigInteger R, BigInteger S)
    {
        public override string ToString()
        {
            return string.Format("{0}{1}", R.ToString(16).PadLeft(64,'0'), S.ToString(16).PadLeft(64, '0'));
        }
    }
}
