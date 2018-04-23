using BulkUninstall.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUninstall.Core
{
    public interface IUninstaller
    {
        IEnumerable<Software> GetInstalledSoftware();
        void Uninstall(IEnumerable<Software> programs);
    }
}
