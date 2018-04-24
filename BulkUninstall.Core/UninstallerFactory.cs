using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUninstall.Core
{
    public class UninstallerFactory
    {
        public static IUninstaller Create()
        {
            return new Win32_Product_ManagementObjectUninstaller();
        }
    }
}
