using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ardent.DataGrid
{
    public class DataGridViewModel
    {
        public Type Type { get; set; }
        public int PageCount { get; set; }
        public string IdentifierColumn { get; set; }
        public List<string> Columns { get; set; }
        public List<int> ColumnWidths { get; set; }
        public string EditLinkFormat { get; set; }
        public string EditLinkText { get; set; }

        public string ColumnsString
        {
            get
            {
                var strReturn = string.Empty;
                for (int i = 0; i < Columns.Count; i++)
                {
                    strReturn += Columns[i] + (i != (Columns.Count - 1) ? "," : "");
                }
                if (!string.IsNullOrWhiteSpace(IdentifierColumn) && !Columns.Contains(IdentifierColumn))
                {
                    strReturn += "," + IdentifierColumn;
                }
                return strReturn;
            }
        }

        public DataGridViewModel()
        {
            EditLinkFormat = "Edit/{0}";
            EditLinkText = "Edit";
            Columns = new List<string>();
            ColumnWidths = new List<int>();
            PageCount = 20; //default
        }

        public string GetIdentifierColumn()
        {
            return string.IsNullOrWhiteSpace(IdentifierColumn) ? Columns[0] : IdentifierColumn;
        }

        public int? GetColumnWidth(int index)
        {
            if (ColumnWidths.Count > index)
            {
                return ColumnWidths[index];
            }
            else
            {
                return null;
            }
        }
    }
}