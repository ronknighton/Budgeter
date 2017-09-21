using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Budgeter.Models;
using Microsoft.AspNet.Identity;

namespace Budgeter.Helpers
{
    public class NotificationsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void CreateNotification(IdentityMessage message)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == message.Destination);
            var notification = new Notification();
            if (user != null)
            {
                notification.Message = message.Body;
                notification.Created = DateTimeOffset.Now;
                notification.Sender = "MMS Notifications";
                notification.RecipientId = user.Id;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }

        }

        public ICollection<Notification> GetAllUserNotifications(string userId)
        {
            var user = db.Users.Find(userId);
            if (user == null)
            {
                return new List<Notification>();
            }
            return db.Notifications.Where(n => n.RecipientId == user.Id).OrderByDescending(n => n.Created).ToList();
        }

        public ICollection<Notification> GetUserNotifications()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user == null)
            {
                return new List<Notification>();
            }
            return db.Notifications.Where(n => n.RecipientId == user.Id && !n.Read).OrderByDescending(n => n.Created).ToList();
        }


        public int GetUserNotificationsCount()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user == null)
            {
                return new List<Notification>().Count;
            }
            return db.Notifications.Where(n => n.RecipientId == user.Id && !n.Read).ToList().Count;
        }
    }
}