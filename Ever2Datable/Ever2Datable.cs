using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever2Datable
{
    public class Converter<T>
    {
        public static DataTable Convert2(IEnumerable<T> entities)
        {
            if (entities == null || entities.Count() == 0)
            {
                return null;
            }

            Type type = entities.FirstOrDefault().GetType();
            var properties = type.GetProperties();
            if (properties == null || properties.Count() == 0)
            {
                return null;
            }

            var columns = properties.Select(s => new DataColumn(s.Name));
            DataTable dt = new DataTable(type.Name, type.Namespace);
            dt.Columns.AddRange(columns.ToArray());

            foreach (var item in entities)
            {
                var newRow = dt.NewRow();
                foreach (var property in properties)
                {
                    newRow[property.Name] = property.GetValue(item);
                }
                dt.Rows.Add(newRow);
            }
            return dt;
        }
    }
}
