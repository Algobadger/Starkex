using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using Org.BouncyCastle.Math;
using Starkex.Converters;
using Starkex.Hashing;
using Starkex.Models;

namespace Starkex.Signatures
{

    public class StarkSigner
    {
        private const string TestStruct = "dYdX(string action)";
        private const string MainStruct = "dYdX(string action,string onlySignOn)";
        private const string DeriveStarkKeyAction = "dYdX STARK Key";
        private const string DeriveAPIKeyAction = "dYdX Onboarding";
        private const string MainNetDomain = "https://trade.dydx.exchange";
        private const string DomainContract = "EIP712Domain(string name,string version,uint256 chainId)";
        private const string Domain = "dYdX";
        private const string Version = "1.0";
        private const string EIP = "0x1901";

        private readonly StarkHashCalculator STARK_HASH_CALCULATOR = new(new PedersonHash(ConstantPoints.GetECPoint(0)));
        private readonly EcSigner EC_SIGNER = new(StarkCurve.GetInstance());

        public Signature Sign(OrderWithClientId order, NetworkId networkId, string privateKey)
        {
            if (privateKey.StartsWith("0x") || privateKey.StartsWith("0X"))
            {
                privateKey = privateKey[2..];
            }
            var key = new BigInteger(privateKey,16);
            var starkOrder = StarkwareOrderConverter.FromOrderWithClientId(order, networkId);
            var hash = STARK_HASH_CALCULATOR.CalculateHash(starkOrder);
            return EC_SIGNER.Sign(key, hash);
        }

        public string DeriveStarkKey(string ethPrivateKey, int networkId)
        {
            var typedSign = GenerateSignature(ethPrivateKey, networkId, DeriveStarkKeyAction);
            var signatureInt = Sha3Keccack.Current.CalculateHashFromHex(typedSign);
            var big = new BigInteger(signatureInt, 16).ShiftRight(5);
            var starkKey = big.ToString(16);

            return "0x" + starkKey;
        }

        public StarkAPI DeriveApiKey(string ethPrivateKey, int networkId)
        {
            var typedSign = GenerateSignature(ethPrivateKey, networkId, DeriveAPIKeyAction);
            var rHex = typedSign.Substring(0, 64);
            var sHex = typedSign.Substring(64, 64);

            var rHash = Sha3Keccack.Current.CalculateHashFromHex(rHex);
            var sHash = Sha3Keccack.Current.CalculateHashFromHex(sHex);

            var secretBytes = rHash[..60];
            var keyBytes = sHash[..32];
            var pass = sHash.Substring(32, 30);

            _ = Guid.TryParse(keyBytes, out Guid res);

            var secret = UrlSafeEncode(Convert.FromHexString(secretBytes));
            var password =UrlSafeEncode(Convert.FromHexString(pass));
            return new StarkAPI(secret, res.ToString(), password);
        }

        private static string GenerateSignature(string ethPrivateKey, int networkId, string action)
        {
            //"0x45ae2c9f941548590b88af98fafd898603592c5725b4606150e1f1bca4d3a134";
            var structString = networkId == 1 ? MainStruct : TestStruct;
            var allStruct = new List<string>
            {
                Sha3Keccack.Current.CalculateHash(structString),
                Sha3Keccack.Current.CalculateHash(action)
            };
            if (networkId == 1)
            {
                allStruct.Add(Sha3Keccack.Current.CalculateHash(MainNetDomain));
            }

            var structhash = Sha3Keccack.Current.CalculateHashFromHex(allStruct.ToArray());


            var contract = Sha3Keccack.Current.CalculateHash(DomainContract);
            var domainHash = Sha3Keccack.Current.CalculateHash(Domain);
            var versionHash = Sha3Keccack.Current.CalculateHash(Version);
            var network = networkId.ToString().PadLeft(64, '0');

            var domain = Sha3Keccack.Current.CalculateHashFromHex(contract, domainHash, versionHash, network);
            var eipHash = Sha3Keccack.Current.CalculateHashFromHex(EIP, domain, structhash);

            var eth = new EthECKey(ethPrivateKey);
            var signed = eth.SignAndCalculateV(Convert.FromHexString(eipHash));

            var rawSign = signed.R.ToHex() + signed.S.ToHex() + signed.V.ToHex();
            var typedSign = CreateTypedSign(rawSign, 0);

            return typedSign;
           
        }

        private static string UrlSafeEncode(byte[] message)
        {
           return  Convert.ToBase64String(message)
                          .TrimEnd('=')
                          .Replace('+', '-')
                          .Replace('/', '_');
        }

        private static string CreateTypedSign(string sign, int type)
        {
            if (sign.Length != 130)
            {
                throw new Exception("Invalid Signature");
            }
            var rs = sign[..128];
            var v = sign.Substring(128, 2);

            if (v == "00")
            {
                return rs + "1b" + $"0{type}";
            }
            else if (v == "01")
            {
                return rs + "1c" + $"0{type}";
            }
            else if (v == "1b" || v == "1c")
            {
                return sign + $"0{type}";
            }
            throw new Exception("Invalid Signature V");
        }
    }
}
