using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Math;
using Starkex.Converters;
using Starkex.Hashing;
using Starkex.Models;
using System;

namespace Starkex.Test
{
    [TestClass]
    public class TestStarkHashCalculator
    {
        private readonly static string HASH_VALUE = "1690222932b6b9f7ec1a92f3950e5332892789e7531336114f588ed08de3a42";
        private static readonly DateTime Time = DateTime.Parse("2021-09-20T00:00:00.000Z").ToUniversalTime();
        private static readonly DydxAsset ATOM = new("ATOM", new BigInteger("41544f4d2d37000000000000000000", 16), 7);
        private static readonly DydxAsset ETH = new("ETH", new BigInteger("4554482d3900000000000000000000", 16), 9);

        private readonly static StarkHashCalculator STARK_HASH_CALCULATOR = new(new PedersonHash(ConstantPoints.GetECPoint(0)));
        private readonly static OrderWithClientIdWithPrice order = new(
                new Order("56277", 1, 0.001, ATOM, StarkwareOrderSide.BUY, Time), "123456", 34.00);

        [TestMethod]
        public void TestHashWithPrice()
        {
            StarkwareOrder starkwareOrder = StarkwareOrderConverter.FromOrderWithClientId(order, NetworkId.ROPSTEN);
            Assert.AreEqual(HASH_VALUE, STARK_HASH_CALCULATOR.CalculateHash(starkwareOrder).ToString(16));
        }

        [TestMethod]
        public void TestHashWithQuateAmount()
        {
            OrderWithClientIdAndQuoteAmount order = new(
                    new Order("56277",
                            1,
                            0.001,
                            ATOM,
                            StarkwareOrderSide.BUY,
                            Time),
                    "123456",
                    34.00
            );

            StarkwareOrder starkwareOrder = StarkwareOrderConverter.FromOrderWithClientId(order, NetworkId.ROPSTEN);
            Assert.AreEqual(HASH_VALUE, STARK_HASH_CALCULATOR.CalculateHash(starkwareOrder).ToString(16));
        }

        [TestMethod]
        public void TestNonce()
        {
            Assert.AreEqual(new BigInteger("987524242"), StarkwareOrderConverter.NonceFromClientId("123456"));
        }

        [TestMethod]
        public void TestHashETH()
        {
            var hash = "54defe3d7784789849556377433b4160f9eecd0ebb450cf3cdc02cb948abf48";
            Order order = new("12345", 145.0005, 0.125, ETH, StarkwareOrderSide.BUY, DateTime.Parse("2020-09-17T04:15:55.028Z").ToUniversalTime());
            OrderWithClientId orderWithClientID = new OrderWithClientIdWithPrice(order,
                    "This is an ID that the client came up with to describe this order", 350.00067);
            StarkwareOrder starkwareOrder = StarkwareOrderConverter.FromOrderWithClientId(orderWithClientID, NetworkId.ROPSTEN);
            Assert.AreEqual(hash, STARK_HASH_CALCULATOR.CalculateHash(starkwareOrder).ToString(16));

        }
    }
}
