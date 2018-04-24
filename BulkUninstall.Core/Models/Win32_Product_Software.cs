using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUninstall.Core.Models
{
    internal class Win32_Product_Software : Software
    {
        internal override void Uninstall()
        {
            this.ManagementObj?.InvokeMethod("Uninstall", null);
        }
    }
}
