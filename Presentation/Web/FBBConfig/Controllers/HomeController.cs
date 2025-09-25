using FBBConfig.Extensions;
using System.Web.Mvc;
using System.Web.UI;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Account;



namespace FBBConfig.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class HomeController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;

        public HomeController(ILogger logger,
            IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        [AuthorizeUserAttribute]
        public ActionResult Index()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            Session["ProgramVersion"] = GetVersion();

            return View();
        }

        public ActionResult Demo()
        {
            var query = new GetUserDataQuery
            {
                UserName = "thitimaw"
            };

            base.CurrentUser = _queryProcessor.Execute(query);
            ViewBag.User = base.CurrentUser;

            return View();
        }

        private string GetVersion()
        {
            var version = "Ver. R16.1.26.26012015";

            var query = new WBBContract.Queries.Commons.Master.GetVersionQuery
            {

            };

            var versionModel = _queryProcessor.Execute(query);

            version = version + "-" + versionModel.InternalServiceVersion;

            return version;
        }


        //public static DataTable getDataTableFromExcel(string path)
        //{ 
        //     var fs = System.IO.File.OpenRead(filePath);
        //    using (var pck = new OfficeOpenXml.ExcelPackage())
        //    {
        //        using (var stream = File.OpenRead(path))
        //        {
        //            pck.Load(stream);
        //        }
        //        var ws = pck.Workbook.Worksheets.First();
        //        DataTable tbl = new DataTable();
        //        bool hasHeader = true; // adjust it accordingly( i've mentioned that this is a simple approach)
        //        foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
        //        {
        //            tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
        //        }
        //        var startRow = hasHeader ? 2 : 1;
        //        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
        //        {
        //            var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
        //            var row = tbl.NewRow();
        //            foreach (var cell in wsRow)
        //            {
        //                row[cell.Start.Column - 1] = cell.Text;
        //            }
        //            tbl.Rows.Add(row);
        //        }
        //        return tbl;
        //    }
        //}


    }
}
