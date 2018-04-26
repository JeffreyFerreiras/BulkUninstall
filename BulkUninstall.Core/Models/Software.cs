using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace BulkUninstall.Core.Models
{
    public abstract class Software
    {
        public string Name { get; set; } = string.Empty;

        public DateTime? InstalledDate { get; set; } = null;

        public string Version { get; set; } = string.Empty;

        public string ID { get; set; } = string.Empty;

        internal ManagementObject ManagementObj { get; set; }

        internal abstract void Uninstall();
    }
}
