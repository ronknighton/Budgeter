using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Invitation
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Code { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTimeOffset Created { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTimeOffset Expires { get; set; }

        //FK's 
        public string SenderId { get; set; }
        public int HouseholdId { get; set; }

        public virtual ApplicationUser Sender { get; set; }
        public virtual Household Household { get; set; }


        public Invitation()
        {

        }

        public Invitation(string email, int hId)
        {
            Email = email;
            HouseholdId = hId;
            Code = Guid.NewGuid().ToString();
            Created = DateTimeOffset.Now;
            Expires = Created.AddDays(7);
            SenderId = HttpContext.Current.User.Identity.GetUserId();
        }



        
    }
}