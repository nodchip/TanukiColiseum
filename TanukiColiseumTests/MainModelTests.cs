using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TanukiColiseum;

namespace TanukiColiseumTests
{
    [TestClass]
    public class MainModelTests
    {
        private const int HashMb = 1024;

        [TestMethod]
        public void Serialize()
        {
            var tempFilePath = Path.GetTempFileName();
            var model = new MainModel();
            model.HashMb.Value = HashMb;

            model.Save(tempFilePath);
            var loaded = new MainModel();
            loaded.Load(tempFilePath);

            Assert.AreEqual(HashMb, loaded.HashMb.Value);

            File.Delete(tempFilePath);
        }
    }
}
