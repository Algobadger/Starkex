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

        private static readonly string EXPECTED_SIGNATURE = "05b4d194ef333605d4945a4c3642879e07062d95449f00bf7749909e9b7afc92033f77986a83420cf6b0e7be90a13b9bfa8489a363e9b51727bd09fe909af60f";
        private static readonly Order order = new("12345", 145.0005, 0.125, ETH, StarkwareOrderSide.BUY, Time);
        private static readonly OrderWithClientId orderWithClientID = new OrderWithClientIdWithPrice(order,
                "This is an ID that the client came up with to describe this order", 350.00067);
        private readonly string PRIVATE_KEY = "07230d8f6fcba9afb8eea3aa67119b5a1bc117500186c384b5aaee85dafbb64c";
        private readonly string privateKey = "0x58c7d5a90b1776bde86ebac077e053ed85b0f7164f53b080304a531947f46e3";
        private readonly string mockSignature = "07670488d9d2c6ff980ca86e6d05b89414de0f2bfd462a1058fb05add68d034a036268ae33e8e21d324e975678f56b66dacb2502a7de1512a46b96fc0e106f79";

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
            Signature signature = starkSigner.Sign(order, NetworkId.GOERLI, PRIVATE_KEY);
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
            Signature signature = starkSigner.Sign(order, NetworkId.MAINNET, "515282606d04ca75bfda0b2f3f92f9ad591f0153ff79ccac99f0a61016a4c5");
            Assert.AreEqual(sign, signature.ToString());
        }

        [TestMethod]
        public void TestSingOrderOddY()
        {
            StarkSigner starkSigner = new();
            Signature signature = starkSigner.Sign(orderWithClientID, NetworkId.GOERLI, privateKey);
            Assert.AreEqual(mockSignature, signature.ToString());
        }

        [TestMethod]
        public void TestSingOrderEvenY()
        {
            string sign = "0618bcd2a8a027cf407116f88f2fa0d866154ee421cdf8a9deca0fecfda5277b03e42fa1d039522fc77c23906253e537cc5b2f392dba6f2dbb35d51cbe37273a";
            StarkSigner starkSigner = new();
            Signature signature = starkSigner.Sign(orderWithClientID, NetworkId.GOERLI, "65b7bb244e019b45a521ef990fb8a002f76695d1fc6c1e31911680f2ed78b84");
            Assert.AreEqual(sign, signature.ToString());
        }

        [TestMethod]
        public void TestOrderQuoteAmount()
        {
            OrderWithClientId orderWithClientID = new OrderWithClientIdAndQuoteAmount(order,
                    "This is an ID that the client came up with to describe this order", 50750.272151);
            StarkSigner starkSigner = new();
            Signature signature = starkSigner.Sign(orderWithClientID, NetworkId.GOERLI, privateKey);
            Assert.AreEqual(mockSignature, signature.ToString());
        }

    }
}
