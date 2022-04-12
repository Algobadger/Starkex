using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Starkex.Signatures
{
    public class EcSigner
    {
        private readonly StarkCurve curve;

        public EcSigner(StarkCurve curve)
        {
            this.curve = curve;
        }

        private static byte[] ToByteArray(BigInteger value)
        {
            byte[] signedValue = value.ToByteArray();
            if (signedValue[0] != 0x00)
            {
                return signedValue;
            }
            return Arrays.CopyOfRange(signedValue, 1, signedValue.Length);
        }

        /**
         * @param privateKey private key
         * @param message    message to be signed
         * @return signed message (ecdsa signature)
         */
        public Signature Sign(BigInteger privateKey, BigInteger message)
        {
            var hMacDSAKCalculator = new HMacDsaKCalculator(new Sha256Digest());
            var signer = new ECDsaSigner(hMacDSAKCalculator);
            signer.Init(true, curve.CreatePrivateKeyParams(privateKey));
            BigInteger[] signature = signer.GenerateSignature(ToByteArray(FixMessageLength(message)));
            return new Signature(signature[0], signature[1]);
        }

        private static BigInteger FixMessageLength(BigInteger message)
        {
            string hashHex = message.ToString(16);
            if (hashHex.Length <= 62)
            {
                // In this case, messageHash should not be transformed, as the byteLength() is at most 31,
                // so delta < 0 (see _truncateToN).
                return message;
            }
            if (hashHex.Length != 63)
            {
                throw new Exception("invalidHashLength " + hashHex.Length);
            }
            // In this case delta will be 4, so we perform a shift-left of 4 bits.
            return message.ShiftLeft(4);
        }

        public bool VerifySignature(BigInteger message, BigInteger publicKey, Signature signature)
        {
            var signer = new ECDsaSigner();
            var publicKeyParameters = curve.CreatePublicKeyParams(publicKey);
            signer.Init(false, publicKeyParameters);
            return signer.VerifySignature(FixMessageLength(message).ToByteArray(), signature.R, signature.S);
        }


    }
}
