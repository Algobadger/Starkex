using Microsoft.VisualStudio.TestTools.UnitTesting;
using Starkex.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starkex.Test
{
    [TestClass]
    public class TestStarkKey
    {
        private const string ExpectedTestStarkKey = "0x6c87e4377a6a8c3feeddc1afe52696daa1209720f720e853016e426d5137c60";
        private const string ExpectedMainStarkKey = "0x553e90eb0d5eb0ef4f20a3f47fe6980df5a29e8b20bf59bc0e6d645d293af36";
        private const string TestPvtKey = "0xd763c0550b15b05377868499ae2b0a758e0be36f945d00c427d7e17d4ee12143";
        private static readonly StarkAPI TestApi = new("kSBivWhxB2NU5EOirsZsxjbUldGO5A4k_Azm09ff", "8a6ccef4-94d4-ad7e-8db5-1c8fb008ed65", "itJURA491_KiFJjYxQX9");
        private static readonly StarkAPI MainApi = new("qUJoY7FvpTSYgF4vgXrTeH40o6IiHoqJ9EntqgXC", "cfe71fc1-2ff3-c635-e9a9-418123a42210", "BjAYnl5BkU6Q_41amxjv");

        [TestMethod]
        public void DeriveTestKey()
        {
            var starkSigner = new StarkSigner();
            var testKey = starkSigner.DeriveStarkKey(TestPvtKey, 3);
            Assert.AreEqual(ExpectedTestStarkKey, testKey);
        }

        [TestMethod]
        public void DeriveMainKey()
        {
            var starkSigner = new StarkSigner();
            var testKey = starkSigner.DeriveStarkKey(TestPvtKey, 1);
            Assert.AreEqual(ExpectedMainStarkKey, testKey);
        }

        [TestMethod]
        public void DeriveTestApI()
        {
            var starkSigner = new StarkSigner();
            var testKey = starkSigner.DeriveApiKey(TestPvtKey, 3);
            Assert.AreEqual(TestApi, testKey);
        }

        [TestMethod]
        public void DeriveMainApI()
        {
            var starkSigner = new StarkSigner();
            var mainKey = starkSigner.DeriveApiKey(TestPvtKey, 1);
            Assert.AreEqual(MainApi, mainKey);
        }
    }
}
