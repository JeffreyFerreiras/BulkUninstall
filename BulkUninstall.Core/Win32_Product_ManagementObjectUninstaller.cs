﻿using System;
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
    internal class Win32_Product_ManagementObjectUninstaller : IUninstaller
    {
        List<ManagementObject> _managementObjects;
        List<Software> _programs;

        public void Uninstall(IEnumerable<Software> programs)
        {
            foreach (Software program in programs)
            {
                program.Uninstall();
            }
        }

        public IEnumerable<Software> GetInstalledSoftware()
        {
            if (_programs.IsValid())
            {
                return _programs;
            }

            _managementObjects = GetManagementObjects("SELECT * FROM Win32_Product");
            _programs = GetProgramsFrom(_managementObjects);

            return _programs;
        }

        private List<Software> GetProgramsFrom(List<ManagementObject> managementObjects)
        {
            var programs = new List<Software>();

            foreach(ManagementObject mo in managementObjects)
            {
                programs.Add(new Win32_Product_Software
                {
                    Name = mo["Name"] as string,
                    Version = mo["Version"] as string,
                    InstalledDate = GetDateTime(mo["InstallDate"]),
                    ManagementObj = mo
                });
            }

            return programs;
        }

        private DateTime? GetDateTime(object date)
        {
            try
            {
                int dateNum = date.ToInt32();

                return dateNum.ToDateTime();
            }
            catch
            {
                return null;
            }
        }

        private List<ManagementObject> GetManagementObjects(string query)
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