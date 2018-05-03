using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkUninstall;
using BulkUninstall.Core;

namespace BulkUninstallTests.UnitTests
{
    [TestFixture]
    public class RegistryUninstallerTests
    {

        private IUninstaller Factory()
        {
            return UninstallerFactory.Create(UninstallEngine.Registry);
        }

        [Test]
        public void GetInstalledSoftware_ValidMachine_ReturnsInstalledSoftware()
        {
            var uninstaller = Factory();
            var installedSoftware = uninstaller.GetInstalledSoftware();

            Assert.IsTrue(installedSoftware.Any());
        }
    }
}
