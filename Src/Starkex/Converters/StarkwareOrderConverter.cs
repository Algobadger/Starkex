using Org.BouncyCastle.Math;
using Starkex.Models;
using System.Security.Cryptography;
using System.Text;

namespace Starkex.Converters
{
    public class StarkwareOrderConverter
    {
        private const int ONE_HOUR_IN_SECONDS = 60 * 60;
        private const int STARK_ORDER_SIGNATURE_EXPIRATION_BUFFER_HOURS = 24 * 7;
        private static readonly BigInteger MAX_NONCE = BigInteger.Two.Pow(32);
        private static readonly DateTime epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /**
          * Creates StarkwareOrder from an OrderWithClientId and networkId
          *
          * @param order     with type OrderWithClientId
          * @param networkId
          * @return starkware order
          * @throws NoSuchAlgorithmException in case of hash algorithm fails
          * @throws QuantumSizeException
          * @see StarkwareOrder,OrderWithClientId,NetworkId
          */
        ///
        public static StarkwareOrder FromOrderWithClientId(OrderWithClientId order, NetworkId networkId)
        {
            BigInteger nonce = NonceFromClientId(order.ClientId);
            if (order is OrderWithClientIdWithPrice orderWithPrice)
            {
                return FromOrderWithNonce(new OrderWithNonceAndPrice(order.Order, nonce, orderWithPrice.HumanPrice), networkId);
            }
            return FromOrderWithNonce(new OrderWithNonceAndQuoteAmount(order.Order, nonce,
                ((OrderWithClientIdAndQuoteAmount)order).HumanQuoteAmount), networkId);
        }

        /**
         * Creates a nonce from clientId
         *
         * @param clientId
         * @return nonce
         * @throws NoSuchAlgorithmException in case of hashing algorithm(sha256) not found
         */
        public static BigInteger NonceFromClientId(string clientId)
        {
            using var mySHA256 = SHA256.Create();
            var hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(clientId));
            var sha256hex = BitConverter.ToString(hash).Replace("-", "");
            return new BigInteger(sha256hex, 16).Mod(MAX_NONCE);
        }

        /**
         * Creates StarkwareOrder from OrderWithNonce and networkId
         *
         * @param order
         * @param networkId
         * @return StarkwareOrder object
         * @throws QuantumSizeException
         * @see OrderWithNonce,StarkwareOrder,NetworkId
         */
        public static StarkwareOrder FromOrderWithNonce(OrderWithNonce order, NetworkId networkId)
        {
            // Need to be careful that the (size, price) -> (amountBuy, amountSell) function is
            // well-defined and applied consistently.
            StarkwareAmounts starkwareAmounts = GetStarkwareAmounts(order, networkId);

            // The limitFee is a fraction, e.g. 0.01 is a 1% fee. It is always paid in the collateral asset.
            BigInteger quantumsAmountFee = GetStarkwareLimitFeeAmount(order.Order.LimitFee, starkwareAmounts.QuantumsAmountCollateral);

            // Convert to a Unix timestamp (in hours) and add buffer to ensure signature is valid on-chain.
            var expirationEpochHours =
                        IsoTimestampToEpochHours(order.Order.ExpirationIsoTimestamp) + STARK_ORDER_SIGNATURE_EXPIRATION_BUFFER_HOURS;

            return new StarkwareOrder(
                            starkwareAmounts,
                            StarkwareOrderType.LIMIT_ORDER_WITH_FEES,
                            quantumsAmountFee,
                            starkwareAmounts.AssetIdCollateral,
                            order.Order.PositionId,
                            order.Nonce,
                            expirationEpochHours
                    );
        }

        /**
         * Creates StarkwareAmounts from and order (OrderWithNonce) and networkdId
         *
         * @param order
         * @param networkId
         * @return StarkwareAmounts object
         * @throws QuantumSizeException
         */
        private static StarkwareAmounts GetStarkwareAmounts(OrderWithNonce order, NetworkId networkId)
        {
            var syntheticAsset = order.Order.Market;

            var qt = order.ToQuantums();
            // Convert the synthetic amount to Starkware quantums.
            var quantumsAmountSynthetic = order.ToQuantums(order.Order.HumanSize, syntheticAsset, MidpointRounding.ToZero);

            return new StarkwareAmounts(
                    quantumsAmountSynthetic,
                    order.ToQuantums(),
                    syntheticAsset.AssetId,
                    networkId.CollateralAddressId,
                    order.Order.Side == StarkwareOrderSide.BUY);
        }


        private static BigInteger GetStarkwareLimitFeeAmount(double limitFee, BigInteger quantumsAmountCollateral)
        {
            var res = limitFee * quantumsAmountCollateral.LongValue;
            return new BigInteger(Math.Round(res, MidpointRounding.AwayFromZero).ToString());
        }

        private static long IsoTimestampToEpochHours(DateTime isoTimestamp)
        {
            //return 1600316155 / ONE_HOUR_IN_SECONDS;
            var epochTime = (isoTimestamp - epoch).TotalSeconds/ONE_HOUR_IN_SECONDS;
            var dt = (long)Math.Round(epochTime, MidpointRounding.ToPositiveInfinity);
            return dt;
        }
    }
}
