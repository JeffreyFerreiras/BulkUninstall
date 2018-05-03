using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUninstall.Core
{
    public class UninstallerFactory
    {
        public static IUninstaller Create(UninstallEngine uninstallEngine = UninstallEngine.Registry)
        {
            switch (uninstallEngine)
            {
                case UninstallEngine.Registry:
                    return new RegistryUninstaller();
                case UninstallEngine.Win32_Product:
                    return new Win32_Product_ManagementObjectUninstaller();
                default:
                    throw new InvalidOperationException("No uninstall engine chosen");
            }
        }
    }
}
