using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeter.Helpers;
using Budgeter.Models;
using Microsoft.AspNet.Identity;

namespace Budgeter.Controllers
{
   // [RequireHttps]

    public class HomeController : Controller
    {
        private CsvHelper csvHelper = new CsvHelper();
        private ApplicationDbContext db = new ApplicationDbContext();
        private UsersHelper userHelper = new UsersHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadCSV()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name");
            //var transList = db.UploadedTransactions.OrderByDescending(x => x.Created).ToList();
            return View();
        }

        public ActionResult ReadCSV(string CsvFile, int BankAccountId)
        {
            var file = Request.Files["/Uploads/WF-BankStatement.csv"];
            
            var csvFile = csvHelper.ReadCsv(CsvFile);
            //var csvFile = csvHelper.ReadCsv(file);
            
            TempData["CsvData"] = csvFile.OrderByDescending(x => x.Date).ToList();
            return View("TestCSV");
        }

      

        public ActionResult SendToCsv(List<UploadedTransaction> list)
        {
            var thisList = list;
            int x = 5;
            int y = 6;
            int z = x + y;
            return View("TestCSV", list);
        }

        [NoDirectAccess]
        public ActionResult ErrorPage()
        {
            return View();
        }

        [NoDirectAccess]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [NoDirectAccess]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}