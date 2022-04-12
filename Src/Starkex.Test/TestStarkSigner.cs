using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Math;
using Starkex.Models;
using Starkex.Signatures;
using System;

namespace Starkex.Test
{
    [TestClass]
    public class TestStarkSigner
    {
        private static readonly DateTime Time = DateTime.Parse("2020-09-17T04:15:55.028Z").ToUniversalTime();
        private static readonly DydxAsset ETH = new("ETH", new BigInteger("4554482d3900000000000000000000", 16), 9);
        private static readonly DydxAsset ATOM = new("ATOM", new BigInteger("41544f4d2d37000000000000000000", 16), 7);
        private static readonly DydxAsset ZEC = new("ZEC", new BigInteger("5a45432d3800000000000000000000", 16), 8);

        private static readonly string EXPECTED_SIGNATURE = "06bca455438f4e337e11dff3897d85fdd425be2eb51bd0db92f34527965934e7003376e6726048f2bb23a6f89e7a7528c8cd832493431dd6c685148517ec752f";
        private static readonly Order order = new("12345", 145.0005, 0.125, ETH, StarkwareOrderSide.BUY, Time);
        private static readonly OrderWithClientId orderWithClientID = new OrderWithClientIdWithPrice(order,
                "This is an ID that the client came up with to describe this order", 350.00067);
        private readonly BigInteger PRIVATE_KEY = new("07230d8f6fcba9afb8eea3aa67119b5a1bc117500186c384b5aaee85dafbb64c", 16);
        private readonly string privateKey = "58c7d5a90b1776bde86ebac077e053ed85b0f7164f53b080304a531947f46e3";
        private readonly string mockSignature = "00cecbe513ecdbf782cd02b2a5efb03e58d5f63d15f2b840e9bc0029af04e8dd0090b822b16f50b2120e4ea9852b340f7936ff6069d02acca02f2ed03029ace5";

        [TestMethod]
        public void TestOrderWithClientIdAndQuoteAmount()
        {
            OrderWithClientIdAndQuoteAmount order = new(
                    new Order("56277",
                            1.0,
                            0.001,
                            ATOM,
                            StarkwareOrderSide.BUY,
                            DateTime.Parse("2021-09-20T00:00:00.000Z").ToUniversalTime()),
                    "123456",
                    34.00
            );

            var starkSigner = new StarkSigner();
            Signature signature = starkSigner.Sign(order, NetworkId.ROPSTEN, PRIVATE_KEY);
            Assert.AreEqual(EXPECTED_SIGNATURE, signature.ToString());
        }

        [TestMethod]
        public void TestSignOrderWithClientIdWithPrice()
        {
            string sign = "0121b9b648ee938ca403bb865ab26aa442edb9b7f2c40edf7f86aae0b9686429014cf84c77fe3701ea4516a9e77c31890a69ec72f1af0c64cada7cbf14313c58";
            OrderWithClientIdWithPrice order = new(
                    new Order("123456",
                            1.00,
                            0.0015,
                            ZEC,
                            StarkwareOrderSide.SELL,
                            DateTime.Parse("2021-11-03T16:22:23Z").ToUniversalTime()),
                    "WyHPW57ZKGwcie18UbEBGcry2QervYgYSG1Fm6YG",
                    200.0
            );
            StarkSigner starkSigner = new();
            Signature signature = starkSigner.Sign(order, NetworkId.MAINNET, new BigInteger("515282606d04ca75bfda0b2f3f92f9ad591f0153ff79ccac99f0a61016a4c5", 16));
            Assert.AreEqual(sign, signature.ToString());
        }

        [TestMethod]
        public void TestSingOrderOddY()
        {
            StarkSigner starkSigner = new();
            Signature signature = starkSigner.Sign(orderWithClientID, NetworkId.ROPSTEN, new BigInteger(privateKey, 16));
            Assert.AreEqual(mockSignature, signature.ToString());
        }

        [TestMethod]
        public void TestSingOrderEvenY()
        {
            string sign = "00fc0756522d78bef51f70e3981dc4d1e82273f59cdac6bc31c5776baabae6ec0158963bfd45d88a99fb2d6d72c9bbcf90b24c3c0ef2394ad8d05f9d3983443a";
            StarkSigner starkSigner = new();
            Signature signature = starkSigner.Sign(orderWithClientID, NetworkId.ROPSTEN, new BigInteger("65b7bb244e019b45a521ef990fb8a002f76695d1fc6c1e31911680f2ed78b84", 16));
            Assert.AreEqual(sign, signature.ToString());
        }

        [TestMethod]
        public void TestOrderQuoteAmount()
        {
            OrderWithClientId orderWithClientID = new OrderWithClientIdAndQuoteAmount(order,
                    "This is an ID that the client came up with to describe this order", 50750.272151);
            StarkSigner starkSigner = new();
            Signature signature = starkSigner.Sign(orderWithClientID, NetworkId.ROPSTEN, new BigInteger(privateKey, 16));
            Assert.AreEqual(mockSignature, signature.ToString());
        }

    }
}
