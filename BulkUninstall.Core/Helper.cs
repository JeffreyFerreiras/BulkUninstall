using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Extensions.Conversion;

namespace BulkUninstall.Core
{
    static class Helper
    {
        /// <summary>
        /// Converts an integer formated as yyyyDDmm into a datetime or returns null
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(object date)
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
    }
}
