using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    public class DataGridController : Controller
    {
        [HttpPost]
        public ActionResult GetData(string type, string columns, int page, int count, List<string> filter, string sort, string sortdir)
        {
            IQueryable dataItems = null;

            //TODO - Set dataItems to IQueryable

            return Json(Ardent.DataGrid.GridHelper.GetData(dataItems, columns, page, count, filter, sort, sortdir));
        }
    }
}
