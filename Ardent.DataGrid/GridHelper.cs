using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Reflection;
using System.Data.Linq;

namespace Ardent.DataGrid
{
    public static class GridHelper
    {
        public static GridDataModel GetData(DataContext db, string type, string columns, int page, int count, List<string> filter, string sort, string sortdir)
        {
            var ass = db.GetType().Assembly; //haha ass

            Type dbType = ass.GetType(type);

            var items = (IQueryable)db.GetType().GetMethod("GetTable").MakeGenericMethod(dbType).Invoke(db, null);

            return GetData(items, columns, page, count, filter, sort, sortdir);
        }

        public static GridDataModel GetData(IQueryable items, string columns, int page, int count, List<string> filter, string sort, string sortdir)
        {
            var columnsSplit = columns.Split(',').ToList();
            
            var gridData = new GridDataModel();

            //Sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                if (string.IsNullOrWhiteSpace(sortdir))
                {
                    sortdir = "asc";
                }
                items = items.OrderBy(sort + " " + sortdir);
            }

            //Filtering
            if (filter == null)
            {
                filter = new List<string>();
            }
            var filteredData = items.Filter(columnsSplit, filter);

            gridData.TotalRows = filteredData.Count();
            gridData.PageCount = count;
            gridData.TotalPages = Convert.ToInt32(Math.Ceiling((double)gridData.TotalRows / (double)gridData.PageCount));

            if (page < 1)
            {
                page = 1;
            }
            if (page > gridData.TotalPages)
            {
                page = gridData.TotalPages;
            }
            gridData.CurrentPage = page;

            //Paging
            if (gridData.TotalRows > 0)
            {
                filteredData = filteredData.Skip(count * (page - 1)).Take(count).ToList();
            }

            foreach (var item in filteredData)
            {
                gridData.Data.Add(item);
            }

            return gridData;
        }


        private static object GetPropertyFromString(object myObject, string propertyName)
        {
            var nested = propertyName.Contains(".");

            var testPropName = nested ? propertyName.Split('.')[0] : propertyName;

            foreach (PropertyInfo info in myObject.GetType().GetProperties())
            {
                if (info.CanRead && info.Name == testPropName.Replace(" ", ""))
                {
                    var value = info.GetValue(myObject, null);
                    if (nested && value != null)
                    {
                        return GetPropertyFromString(value, propertyName.Substring(propertyName.IndexOf(".") + 1));
                    }
                    else
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        private static List<Dictionary<String, String>> Filter(this IQueryable source, List<string> columns, List<string> filter)
        {

            var rowType = source.ElementType;
            IList<String> nonQueryableColumns = new List<string>();
            for (int i = 0; i < filter.Count; i++)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(filter[i])) continue;
                    
                    //check if the type is nullable
                    var prop = GetPropertyInfo(rowType, columns[i]);
                    var type = prop.PropertyType;

                    if (rowType.GetProperty(columns[i]).GetCustomAttributes(true).Any(r => r.GetType() == typeof(NotDatagridQueryable)))
                    {
                        nonQueryableColumns.Add(columns[i]);
                        continue;
                    }
                        
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //Its a fucking nullable!
                        source = source.Where(columns[i].Replace(" ", "") + ".Value.ToString().Contains(\"" + filter[i].ToString() + "\")");
                    }
                    else
                    {
                        source = source.Where(columns[i].Replace(" ", "") + ".ToString().Contains(\"" + filter[i].ToString() + "\")");
                    }
                }
                catch { }
            }
            var outData = new List<Dictionary<String, String>>();
            foreach (var item in source)
            {
                var itemData = new Dictionary<string, string>();
                Boolean doAdd = true;
                foreach (var column in columns)
                {
                    object property = GetPropertyFromString(item, column.Replace(" ", ""));
                    if (property == null)
                    {
                        property = string.Empty;
                    }

                    if (nonQueryableColumns.Any(c => c == column))
                    {
                        int index = columns.IndexOf(column);
                        if (!String.IsNullOrEmpty(filter[index]) && filter[index] != property.ToString())
                        {
                            doAdd = false;
                        }
                    }
                    itemData.Add(column.Replace(".", "_"), property.ToString());
                }
               if (doAdd) outData.Add(itemData);
            }
            return outData;
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propName)
        {
            var property = type.GetProperty(propName);
            if (property != null)
            {
                return property;
            }
            //try recursion
            var firstProp = type.GetProperty(propName.Substring(0, propName.IndexOf(".")));
            if (firstProp != null)
            {
                return GetPropertyInfo(firstProp.PropertyType, propName.Substring(propName.IndexOf(".") + 1));
            }

            return null;
        }

        
    }
    public class NotDatagridQueryable : System.Attribute
    {}
}
