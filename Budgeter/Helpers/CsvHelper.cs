using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class CsvHelper
    {
        //Not Used - for testing only
        public IEnumerable<CsvTransaction> ReadNotSaveCsv(HttpPostedFileBase file)
        {
            string csvFile = new StreamReader(file.InputStream).ReadToEnd();
            var transList = new List<CsvTransaction>();
            foreach (var row in csvFile)
            {
                var transaction = new CsvTransaction();
                //var column = row.Split(",");
                //if (!String.IsNullOrWhiteSpace(column[0]))
                //{
                //    //column[0].Replace("\"", "");
                //    transaction.Date = Convert.ToDateTime(column[0]);
                //    //transaction.Date = DateTime.ParseExact(column[0], @"m/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                //}
                //else
                //{
                //    continue;
                //}
                //if (!String.IsNullOrWhiteSpace(column[1]))
                //{
                //    transaction.Amount = Convert.ToDecimal(column[1]);
                //}
                //else
                //{
                //    continue;
                //}
                //if (!String.IsNullOrWhiteSpace(column[4]))
                //{
                //    transaction.Description = column[4];
                //}
                //else
                //{
                //    continue;
                //}
                transList.Add(transaction);

            }

            return transList;
        }

        public IEnumerable<CsvTransaction> ReadCsv(string fileName)
        {
            var transList = new List<CsvTransaction>();
            CsvTransaction transaction;
            var transactions = File.ReadLines(HttpContext.Current.Server.MapPath(fileName)).ToList();
            
            foreach (var row in transactions)
            {
                transaction = new CsvTransaction();
                var column = row.Split(',');
                if (!String.IsNullOrWhiteSpace(column[0]))
                {
                    var date = column[0].Replace("\"", "");
                    transaction.Date = Convert.ToDateTime(date);                    
                }
                else
                {
                    continue;
                }
                if (!String.IsNullOrWhiteSpace(column[1]))
                {
                    var amount = column[1].Replace("\"", "");
                    transaction.Amount = Convert.ToDecimal(amount);
                }
                else
                {
                    continue;
                }
                if (!String.IsNullOrWhiteSpace(column[4]))
                {
                    var description = column[4].Replace("\"", "");
                    transaction.Description = description;
                }
                else
                {
                    continue;
                }
                transList.Add(transaction);

            }

            return transList;
        }

        public string UploadCsv(HttpPostedFileBase file)
        {
            string filePath = null;

            //var imageSize = 2 * 1024 * 1024;
            if (file != null && file.ContentLength > 0)
            {
                //check the file name to make sure its a .csv
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext == ".csv")
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fullFileName = DateTime.Now.ToString("hh.mm.ss.ffffff") + "_" + fileName;
                    file.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fullFileName));

                    filePath = "/Uploads/" + fullFileName;
                    return filePath;
                }


            }


            return "";
        }

        public void DeleteCsv(string filePath)
        {
            File.Delete(HttpContext.Current.Server.MapPath(filePath));
        }
    }

}