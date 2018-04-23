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
    public class UninstallerTests
    {

        IUninstaller Factory()
        {
            return UninstallerFactory.Create();
        }

        [Test]
        public void TestMethod()
        {
            var uninstaller = Factory();

            Assert.IsTrue(uninstaller.GetInstalledSoftware().Any());
        }
    }
}
