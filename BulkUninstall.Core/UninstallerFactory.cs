using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUninstall.Core
{
    public class UninstallerFactory
    {
        /*
         * In a windows 10 environment all applications appear to be registered.
         * For windows 10, use class Win32_Product_ManagementObjectUninstaller
         * Needs more conclusive research.
         */
        public static IUninstaller Create()
        {
            //return new Registry_Uninstaller();
            return new Win32_Product_ManagementObjectUninstaller();
        }
    }
}
