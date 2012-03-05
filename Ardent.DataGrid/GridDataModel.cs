using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ardent.DataGrid
{
    public class GridDataModel
    {
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public List<Dictionary<string, string>> Data { get; set; }

        public GridDataModel()
        {
            Data = new List<Dictionary<string, string>>();
        }
    }
}
