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
        private readonly static string HASH_VALUE = "2ba8fb745b371a256f6930c71e82ce08aadf7e1fc78ba70a1cab7af1cce78c3";
        private static readonly DateTime Time = DateTime.Parse("2021-09-20T00:00:00.000Z").ToUniversalTime();
        private static readonly DydxAsset ATOM = new("ATOM", new BigInteger("41544f4d2d37000000000000000000", 16), 7);
        private static readonly DydxAsset ETH = new("ETH", new BigInteger("4554482d3900000000000000000000", 16), 9);

        private readonly static StarkHashCalculator STARK_HASH_CALCULATOR = new(new PedersonHash(ConstantPoints.GetECPoint(0)));
        private readonly static OrderWithClientIdWithPrice order = new(
                new Order("56277", 1, 0.001, ATOM, StarkwareOrderSide.BUY, Time), "123456", 34.00);

        [TestMethod]
        public void TestHashWithPrice()
        {
            StarkwareOrder starkwareOrder = StarkwareOrderConverter.FromOrderWithClientId(order, NetworkId.GOERLI);
            var hash = STARK_HASH_CALCULATOR.CalculateHash(starkwareOrder).ToString(16);
            Assert.AreEqual(HASH_VALUE, hash);
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

            StarkwareOrder starkwareOrder = StarkwareOrderConverter.FromOrderWithClientId(order, NetworkId.GOERLI);
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
            var hash = "4ceeb860e17a84bfeab2c7768e51d550f4e6baa760cc3f2f2dd43104aa3e680";
            Order order = new("12345", 145.0005, 0.125, ETH, StarkwareOrderSide.BUY, DateTime.Parse("2020-09-17T04:15:55.028Z").ToUniversalTime());
            OrderWithClientId orderWithClientID = new OrderWithClientIdWithPrice(order,
                    "This is an ID that the client came up with to describe this order", 350.00067);
            StarkwareOrder starkwareOrder = StarkwareOrderConverter.FromOrderWithClientId(orderWithClientID, NetworkId.GOERLI);
            var calcHash = STARK_HASH_CALCULATOR.CalculateHash(starkwareOrder).ToString(16);
            Assert.AreEqual(hash, calcHash);

        }
    }
}
