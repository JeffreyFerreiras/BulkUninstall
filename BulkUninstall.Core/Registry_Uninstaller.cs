using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkUninstall.Core.Models;
using Microsoft.Win32;

namespace BulkUninstall.Core
{
    public class Registry_Uninstaller : IUninstaller
    {


        public Registry_Uninstaller()
        {

        }

        public IEnumerable<Software> GetInstalledSoftware()
        {
            RegistryKey currenUser = Registry.CurrentUser.OpenSubKey("Uninstall");
            RegistryKey hkey64 = Registry.LocalMachine.OpenSubKey("Uninstall");

            List<Software> allInstalled = new List<Software>();

            allInstalled.AddRange(GetSoftwareFromRegistry(currenUser));
            
            return allInstalled;
        }

        private IEnumerable<Software> GetSoftwareFromRegistry(RegistryKey key)
        {
            string[] programRegistryKeyLocation = key.GetSubKeyNames();

            foreach (string keyLoc in programRegistryKeyLocation)
            {
                RegistryKey currentKey = key.OpenSubKey(keyLoc);

                yield return new RegistrySoftware
                {
                    Name = currentKey.GetValue("DisplayName") as string,
                    Version = currentKey.GetValue("Version") as string,
                    InstalledDate = null,
                };
            }
        }

        public void Uninstall(IEnumerable<Software> programs)
        {
            throw new NotImplementedException();
        }
    }
}
