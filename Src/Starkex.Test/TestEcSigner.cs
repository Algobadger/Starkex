using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Math;
using Starkex.Signatures;

namespace Starkex.Test
{
    [TestClass]
    public class TestEcSigner
    {
        private static readonly string EXPECTED_SIGNATURE = "06bca455438f4e337e11dff3897d85fdd425be2eb51bd0db92f34527965934e7003376e6726048f2bb23a6f89e7a7528c8cd832493431dd6c685148517ec752f";
        private static readonly BigInteger PRIVATE_KEY = new("07230d8f6fcba9afb8eea3aa67119b5a1bc117500186c384b5aaee85dafbb64c", 16);
        /*
        MESSAGE is Hash representation of sample order:
          {
        humanSize: "1",
        humanPrice: "34.00",
        limitFee: "0.001",
        market: "ATOM-USD",
        side: "BUY",
        expirationIsoTimestamp: "2021-09-20T00:00:00.000Z",
        clientId : "123456",
        positionId: "56277",
        }
         */
        private static readonly BigInteger MESSAGE = new BigInteger("1690222932b6b9f7ec1a92f3950e5332892789e7531336114f588ed08de3a42", 16);

        [TestMethod]
        public void TestSign()
        {
            StarkCurve curve = StarkCurve.GetInstance();
            EcSigner signer = new(curve);
            Signature signature = signer.Sign(PRIVATE_KEY, MESSAGE);
            Assert.AreEqual(signature.ToString(), EXPECTED_SIGNATURE);
        }

        [TestMethod]
        public void TestVerifySignature()
        {
            StarkCurve curve = StarkCurve.GetInstance();
            EcSigner signer = new(curve);
            Signature signature = signer.Sign(PRIVATE_KEY, MESSAGE);
            Assert.IsTrue(signer.VerifySignature(MESSAGE, curve.GeneratePublicKeyFromPrivateKey(PRIVATE_KEY), signature));
        }
    }
}