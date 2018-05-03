using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkUninstall.Core.Models;
using Microsoft.Win32;
using Tools.Extensions.Conversion;

namespace BulkUninstall.Core
{
    public class RegistryUninstaller : IUninstaller
    {
        //HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\ActiveTouchMeetingClient
        const string Win32Loc = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        //const string Win32Loc = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\USerData\S-1-5-18";     
        const string Win64Loc = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        public RegistryUninstaller()
        {

        }

        public IEnumerable<Software> GetInstalledSoftware()
        {
            List<Software> softwares = new List<Software>();

            softwares.AddRange(Get32And64BitSoftware(Registry.CurrentUser));
            softwares.AddRange(Get32And64BitSoftware(Registry.LocalMachine));

            return softwares;
        }

        private IEnumerable<Software> Get32And64BitSoftware(RegistryKey registry)
        {
            if(registry == null)
            {
                return Enumerable.Empty<Software>();
            }

            RegistryKey hkey32 = registry.OpenSubKey(Win32Loc);
            RegistryKey hkey64 = registry.OpenSubKey(Win64Loc);

            List<Software> softwares = new List<Software>();

            softwares.AddRange(GetSoftwareFromRegistry(hkey32));
            softwares.AddRange(GetSoftwareFromRegistry(hkey64));

            return softwares;
        }

        private IEnumerable<Software> GetSoftwareFromRegistry(RegistryKey key)
        {
            if(key == null)
            {
                return Enumerable.Empty<Software>();
            }

            string[] programRegistryKeyLocations = key.GetSubKeyNames();

            return GetSoftwareFromRegistryFissure();

            IEnumerable<Software> GetSoftwareFromRegistryFissure() //used so yielding happens in the method and validation can happen eagerly.
            {
                foreach (string keyLoc in programRegistryKeyLocations)
                {
                    RegistryKey currentKey = key.OpenSubKey(keyLoc);

                    if (currentKey == null) continue;

                    var software = new RegistrySoftware
                    {
                        Name = GetName(currentKey),
                        Version = currentKey.GetValue("DisplayVersion") as string,
                        InstalledDate = Helper.GetDateTime(currentKey.GetValue("InstallDate")),
                        UninstallString = currentKey.GetValue("UninstallString") as string,
                        InstallLocation = currentKey.GetValue("InstallLocation") as string,
                        CurrentRegistryKey = currentKey,
                        ID = currentKey.ToString()
                    };

                    if (string.IsNullOrWhiteSpace(software.UninstallString))
                    {
                        continue;
                    }

                    yield return software;
                }
            }
        }

        private string GetName(RegistryKey key)
        {
            string name = key.GetValue("DisplayName") as string;

            if (name == null)
            {
                string valuename = key.GetValueNames().FirstOrDefault(n =>
                {
                    return n.IndexOf("Name", StringComparison.CurrentCultureIgnoreCase) > -1;
                });

                if(name != null)
                {
                    name = key.GetValue(valuename) as string;
                }
            }

            return name;
        }

        public void Uninstall(IEnumerable<Software> programs)
        {
            Process p = new Process();
            p.StartInfo.FileName = "msiexec.exe";
            p.StartInfo.Arguments = $"/x \"insert key id here\" /qn";
            p.Start();
        }
    }
}
