using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]        
        [AllowHtml]
        public string Message { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm}")]
        public DateTimeOffset Created { get; set; }

        public string Sender { get; set; }
        public bool Read { get; set; }
        //FK's

        public string RecipientId { get; set; }

        //public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
    }
}