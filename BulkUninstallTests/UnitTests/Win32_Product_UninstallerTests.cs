using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkUninstall.Core;

namespace BulkUninstallTests.UnitTests
{
    [TestFixture]
    public class Win32_Product_UninstallerTests
    {

        IUninstaller Factory()
        {
            return UninstallerFactory.Create(UninstallEngine.Win32_Product);
        }

        [Test]
        public void TestMethod()
        {
            var uninstaller = Factory();

            Assert.IsTrue(uninstaller.GetInstalledSoftware().Any());
        }
    }
}
