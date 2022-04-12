using Org.BouncyCastle.Math;
using Starkex.Models;

namespace Starkex.Hashing
{
    public class StarkHashCalculator : IHashCalculator<StarkwareOrder>
    {
        private const string LIMIT_ORDER_WITH_FEES = "3";
        private const int ORDER_PADDING_BITS = 17;
        private readonly IHashFunction hashFunction;


        public StarkHashCalculator(IHashFunction hashFunction)
        {
            this.hashFunction = hashFunction;
        }
        public BigInteger CalculateHash(StarkwareOrder message)
        {
            BigInteger positionIdBn = new(message.PositionId);
            BigInteger expirationEpochHours = new(message.ExpirationEpochHours.ToString());

            BigInteger assetIdSellBn;
            BigInteger assetIdBuyBn;
            BigInteger quantumsAmountSellBn;
            BigInteger quantumsAmountBuyBn;

            if (message.StarkwareAmounts.IsBuyingSynthetic)
            {
                assetIdSellBn = message.StarkwareAmounts.AssetIdCollateral;
                assetIdBuyBn = message.StarkwareAmounts.AssetIdSynthetic;
                quantumsAmountSellBn = message.StarkwareAmounts.QuantumsAmountCollateral;
                quantumsAmountBuyBn = message.StarkwareAmounts.QuantumsAmountSynthetic;
            }
            else
            {
                assetIdSellBn = message.StarkwareAmounts.AssetIdSynthetic;
                assetIdBuyBn = message.StarkwareAmounts.AssetIdCollateral;
                quantumsAmountSellBn = message.StarkwareAmounts.QuantumsAmountSynthetic;
                quantumsAmountBuyBn = message.StarkwareAmounts.QuantumsAmountCollateral;
            }

            CheckFieldSizes(message, message.AssetIdFee, message.QuantumsAmountFee, message.Nonce, positionIdBn, expirationEpochHours);

            BigInteger orderPart1 = new BigInteger(quantumsAmountSellBn.ToString())
                   .ShiftLeft(OrderFieldBitLengths.QUANTUMS_AMOUNT).Add(quantumsAmountBuyBn)
                   .ShiftLeft(OrderFieldBitLengths.QUANTUMS_AMOUNT).Add(message.QuantumsAmountFee)
                   .ShiftLeft(OrderFieldBitLengths.NONCE).Add(message.Nonce);

            BigInteger orderPart2 = new BigInteger(LIMIT_ORDER_WITH_FEES)
                   .ShiftLeft(OrderFieldBitLengths.POSITION_ID).Add(positionIdBn) // Repeat (1/3).
                   .ShiftLeft(OrderFieldBitLengths.POSITION_ID).Add(positionIdBn) // Repeat (2/3).
                   .ShiftLeft(OrderFieldBitLengths.POSITION_ID).Add(positionIdBn) // Repeat (3/3).
                   .ShiftLeft(OrderFieldBitLengths.EXPIRATION_EPOCH_HOURS).Add(expirationEpochHours)
                   .ShiftLeft(ORDER_PADDING_BITS);

            BigInteger cacheAsset = hashFunction.HashFromCache(assetIdSellBn, assetIdBuyBn);
            BigInteger assetsBn = hashFunction.HashFromCache(cacheAsset, message.AssetIdFee);

            return hashFunction.CreateHash(
                    hashFunction.CreateHash(assetsBn, orderPart1),
                    orderPart2
            );
        }

        private static void CheckFieldSizes(StarkwareOrder message, BigInteger assetIdFee, BigInteger quantumsAmountFee, BigInteger nonce, BigInteger positionId, BigInteger expirationEpochHours)
        {
            if (message.StarkwareAmounts.AssetIdSynthetic.BitLength > OrderFieldBitLengths.ASSET_ID_SYNTHETIC)
            {
                throw new Exception("assetIdSynthetic");
            }
            if (message.StarkwareAmounts.AssetIdCollateral.BitLength > OrderFieldBitLengths.ASSET_ID_COLLATERAL)
            {
                throw new Exception("assetIdCollateral");
            }
            if (assetIdFee.BitLength > OrderFieldBitLengths.ASSET_ID_FEE)
            {
                throw new Exception("assetIdFee");
            }
            if (message.StarkwareAmounts.QuantumsAmountSynthetic.BitLength > OrderFieldBitLengths.QUANTUMS_AMOUNT)
            {
                throw new Exception("quantumsAmountSynthetic");
            }
            if (message.StarkwareAmounts.QuantumsAmountCollateral.BitLength > OrderFieldBitLengths.QUANTUMS_AMOUNT)
            {
                throw new Exception("quantumsAmountCollateral");
            }
            if (quantumsAmountFee.BitLength > OrderFieldBitLengths.QUANTUMS_AMOUNT)
            {
                throw new Exception("quantumsAmountFee");
            }
            if (nonce.BitLength > OrderFieldBitLengths.NONCE)
            {
                throw new Exception("nonce");
            }
            if (positionId.BitLength > OrderFieldBitLengths.POSITION_ID)
            {
                throw new Exception("positionId");
            }
            if (expirationEpochHours.BitLength > OrderFieldBitLengths.EXPIRATION_EPOCH_HOURS)
            {
                throw new Exception("expirationEpochHours");
            }
        }


        static class OrderFieldBitLengths
        {
            public const int ASSET_ID_SYNTHETIC = 128;
            public const int ASSET_ID_COLLATERAL = 250;
            public const int ASSET_ID_FEE = 250;
            public const int QUANTUMS_AMOUNT = 64;
            public const int NONCE = 32;
            public const int POSITION_ID = 64;
            public const int EXPIRATION_EPOCH_HOURS = 32;
        }
    }
}
