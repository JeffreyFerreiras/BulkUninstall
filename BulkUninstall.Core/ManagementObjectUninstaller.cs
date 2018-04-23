using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Management;
using BulkUninstall.Core.Models;
using Tools.Extensions.Validation;
using Tools.Extensions.Conversion;

namespace BulkUninstall.Core
{
    internal class ManagementObjectUninstaller : IUninstaller
    {
        List<ManagementObject> _managementObjects;
        List<Software> _programs;

        public void Uninstall(IEnumerable<Software> programs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Software> GetInstalledSoftware()
        {
            if (_programs.IsValid())
            {
                return _programs;
            }

            _managementObjects = GetManagementObjects();
            _programs = GetProgramsFrom(_managementObjects);

            return _programs;
        }

        private List<Software> GetProgramsFrom(List<ManagementObject> managementObjects)
        {
            var programs = new List<Software>();

            foreach(ManagementObject mo in managementObjects)
            {
                programs.Add(new Software
                {
                    Name = mo["DisplayName"] as string,
                    Version = mo["Version"] as string,
                    InstalledDate = GetDateTime(mo["InstallDate"]),
                });
            }

            return programs;
        }

        private DateTime? GetDateTime(object date)
        {
            try
            {
                int dateNum = date.ToInt32();
                DateTime dt = dateNum.ToDateTime();

                return dt;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private List<ManagementObject> GetManagementObjects(string query = "SELECT * FROM Win32reg_AddRemovePrograms")
        {
            var searcher = new ManagementObjectSearcher(query);
            var moCollection = searcher.Get();
            var managementObjects = new List<ManagementObject>();

            foreach(ManagementObject mo in moCollection)
            {
                managementObjects.Add(mo);
            }

            return managementObjects;
        }
    }
}
