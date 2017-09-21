using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Budgeter.Helpers;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private NotificationsHelper notificationsHelper = new NotificationsHelper();

        // GET: Notifications
        [NoDirectAccess]
        public ActionResult Index()
        {
            var notifications = db.Notifications.Include(n => n.Recipient);
            return View(notifications.ToList());
        }

        [NoDirectAccess]
        public ActionResult UserIndex()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            return View("Index", notificationsHelper.GetAllUserNotifications(user.Id));
        }

        // GET: Notifications/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
           
            if (user.HouseholdId != null)
            {
                var members = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == user.HouseholdId).Members.ToList();
                ViewBag.RecipientId = new SelectList(members, "Id", "FullName");
            }
            else
            {
                ViewBag.NoHousehold = "You must belong to a household to send a message";
            }

            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Message,RecipientId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                notification.Created = DateTimeOffset.Now;
                notification.Sender = user.Id;
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("UserIndex");
            }

            ViewBag.RecipientId = new SelectList(db.Users, "Id", "FirstName", notification.RecipientId);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.RecipientId = new SelectList(db.Users, "Id", "FirstName", notification.RecipientId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Message,Created,Sender,Read,RecipientId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.Read = true;
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserIndex");
            }
            ViewBag.RecipientId = new SelectList(db.Users, "Id", "FirstName", notification.RecipientId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
            db.SaveChanges();
            return RedirectToAction("UserIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
