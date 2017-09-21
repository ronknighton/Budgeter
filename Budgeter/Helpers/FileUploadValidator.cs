using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class FileUploadValidator
    {
        public bool FileChecker(HttpPostedFileBase image)
        {
            var imageSize = 2 * 1024 * 1024;
            if (image != null && image.ContentLength > 0 && image.ContentLength < imageSize)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext != ".bmp")
                {
                    return true;
                }

            }
            return false;

        }

        public bool IsImagefile(string mediaUrl)
        {
            var ext = Path.GetExtension(mediaUrl).ToLower();
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp")
            {
                return true;
            }
            return false;
        }

        public bool IsDisplayFile(string mediaUrl)
        {
            var ext = Path.GetExtension(mediaUrl).ToLower();
            if (ext == ".pdf" || ext == ".txt")
            {
                return true;
            }
            return false;
        }

        public bool IsNotDisplayFile(string mediaUrl)
        {
            var ext = Path.GetExtension(mediaUrl).ToLower();
            if (ext == ".doc" || ext == ".docx" || ext == ".xls" || ext == ".xlsx")
            {
                return true;
            }
            return false;
        }





        public string ValidateAndSaveFile(HttpPostedFileBase image)
        {
            string filePath = null;
            var imageSize = 2 * 1024 * 1024;
            if (image != null && image.ContentLength > 0 && image.ContentLength < imageSize)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext != ".bmp")
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fileName));
                    filePath = "/Uploads/" + fileName;
                    return filePath;
                }

            }


            return "";
        }

        public string ValidateAndSaveFile2(HttpPostedFileBase image)
        {
            string filePath = null;
            var lowerFileSize = 3 * 1024;
            var upperFileSize = 2 * 1024 * 1024;


            if (image == null)
            {
                return "null";
            }
            var ext = Path.GetExtension(image.FileName).ToLower();

            if (image != null && image.ContentLength < lowerFileSize)
            {
                return "small";
            }

            if (image.ContentLength > upperFileSize)
            {
                return "large";
            }

            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp" || ext == ".pdf" || ext == ".doc" || ext == ".docx" || ext == ".txt" || ext == ".xlsx" || ext == ".xls" )
            {
                var fileName = Path.GetFileName(image.FileName);
                var fullFileName = DateTime.Now.ToString("hh.mm.ss.ffffff") + "_" + fileName;
                image.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fullFileName));
                filePath = "/Uploads/" + fullFileName;
                return filePath;
            }

            return "format";

        }

        public string ValidateAndSaveFile3(HttpPostedFileBase image)
        {
            string filePath = null;
            var lowerFileSize = 1024;
            var upperFileSize = 1 * 1024 * 1024;


            if (image == null)
            {
                return "null";
            }
            var ext = Path.GetExtension(image.FileName).ToLower();

            if (image != null && image.ContentLength < lowerFileSize)
            {
                return "small";
            }

            if (image.ContentLength > upperFileSize)
            {
                return "large";
            }
         
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp")
            {
                var fileName = Path.GetFileName(image.FileName);
                var fullFileName = DateTime.Now.ToString("hh.mm.ss.ffffff") + "_" + fileName;
                image.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fullFileName));
                filePath = "/Uploads/" + fullFileName;
                return filePath;
            }

            return "format";

        }

    }
}