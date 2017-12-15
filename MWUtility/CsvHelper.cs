using MWUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWUtility
{
    public static class CsvHelper
    {
        public static int Convert2Csv<T>(this IEnumerable<T> entities, string filePath)
        {
            var dt = Ever2Datable<T>.Convert2(entities);
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.AppendAllText(filePath, sb.ToString());
            return entities.Count();
        }

        public static int Convert2Csv<T>(this IEnumerable<T> entities)
        {
            string filePath = "Csv_" + DateTime.Now.ToString("yyyyMMddHHmmss")+".csv";
            return Convert2Csv(entities, filePath);
        }

    }
}
