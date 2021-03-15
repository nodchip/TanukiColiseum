using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TanukiColiseum;

namespace TanukiColiseumTests
{
    [TestClass]
    public class OptionsTests
    {
        [TestMethod]
        public void RemoveCommonPath_Same()
        {
            string path1 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs";
            string path2 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs";
            Options.RemoveCommonPath(path1, path2, out string outputPath1, out string outputPath2);
            Assert.AreEqual(@"MainModel.cs", outputPath1);
            Assert.AreEqual(@"MainModel.cs", outputPath2);
        }

        [TestMethod]
        public void RemoveCommonPath_Path1IsLonger()
        {
            string path1 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum";
            string path2 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs";
            Options.RemoveCommonPath(path1, path2, out string outputPath1, out string outputPath2);
            Assert.AreEqual(@"TanukiColiseum", outputPath1);
            Assert.AreEqual(@"TanukiColiseum\MainModel.cs", outputPath2);
        }

        [TestMethod]
        public void RemoveCommonPath_Path2IsLonger()
        {
            string path1 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs";
            string path2 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum";
            Options.RemoveCommonPath(path1, path2, out string outputPath1, out string outputPath2);
            Assert.AreEqual(@"TanukiColiseum\MainModel.cs", outputPath1);
            Assert.AreEqual(@"TanukiColiseum", outputPath2);
        }

        [TestMethod]
        public void RemoveCommonPath_SuffixIsDifferent()
        {
            string path1 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs";
            string path2 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseumTests\MainModelTests.cs";
            Options.RemoveCommonPath(path1, path2, out string outputPath1, out string outputPath2);
            Assert.AreEqual(@"TanukiColiseum\MainModel.cs", outputPath1);
            Assert.AreEqual(@"TanukiColiseumTests\MainModelTests.cs", outputPath2);
        }

        [TestMethod]
        public void RemoveCommonPath_RootPathIsDifferent()
        {
            string path1 = @"C:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs";
            string path2 = @"D:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs";
            Options.RemoveCommonPath(path1, path2, out string outputPath1, out string outputPath2);
            Assert.AreEqual(@"C:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs", outputPath1);
            Assert.AreEqual(@"D:\home\nodchip\TanukiColiseum\TanukiColiseum\MainModel.cs", outputPath2);
        }
    }
}
