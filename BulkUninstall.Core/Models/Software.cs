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
        public string Name { get; set; }

        public DateTime? InstalledDate { get; set; }

        public string Version { get; set; }

        public string ID { get; set; }

        internal ManagementObject ManagementObj { get; set; }

        internal abstract void Uninstall();
    }
}
