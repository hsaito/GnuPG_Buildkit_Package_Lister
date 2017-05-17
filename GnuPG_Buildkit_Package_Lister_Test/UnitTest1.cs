using Microsoft.VisualStudio.TestTools.UnitTesting;
using GnuPG_Buildkit_Package_Lister;

namespace GnuPG_Buildkit_Package_Lister_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FetchWeb()
        {
            var processor = new GnuPG_Processor();
            processor.GetWeb("http://example.com").Wait();
        }
    }
}
