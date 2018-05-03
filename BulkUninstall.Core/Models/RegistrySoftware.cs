using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BulkUninstall.Core.Models
{

    internal class RegistrySoftware : Software
    {
        public string UninstallString { get; set; }
        public string InstallLocation { get; internal set; }
        public RegistryKey CurrentRegistryKey { get; internal set; }

        internal override void Uninstall()
        {
            throw new NotImplementedException();
        }
    }
}
