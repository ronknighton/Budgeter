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
using System.Threading.Tasks;
using Budgeter.Helpers;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UsersHelper usersHelper = new UsersHelper();

        // GET: Invitations
        [NoDirectAccess]
        public ActionResult Index()
        {
            var invitations = db.Invitations.Include(i => i.Household).Include(i => i.Sender);
            return View(invitations.ToList());
        }

        // GET: Invitations/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // GET: Invitations/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            ViewBag.SenderId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Created,Expires,SenderId,HouseholdId")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Invitations.Add(invitation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitation.HouseholdId);
            ViewBag.SenderId = new SelectList(db.Users, "Id", "FirstName", invitation.SenderId);
            return View(invitation);
        }

        // GET: Invitations/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitation.HouseholdId);
            ViewBag.SenderId = new SelectList(db.Users, "Id", "FirstName", invitation.SenderId);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Created,Expires,SenderId,HouseholdId")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitation.HouseholdId);
            ViewBag.SenderId = new SelectList(db.Users, "Id", "FirstName", invitation.SenderId);
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            db.Invitations.Remove(invitation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Send/Confirm Invites

        //GET: Send Invitation
        [NoDirectAccess]
        public ActionResult SendInvitation(int hId)
        {
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            ViewBag.HouseholdId = hId;
            ViewBag.HouseholdName = db.Households.FirstOrDefault(h => h.Id == hId).Name;
            return View();
        }

        [HttpPost]
        [NoDirectAccess]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendInvitation([Bind(Include = "Email,HouseholdId")]SendInviteViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == null || model.HouseholdId == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ViewBag.HouseholdId = model.HouseholdId;
                    TempData["error"] = "Something went wrong with the invite. Please try again";
                    return View(model);
                }

                var user = db.Users.FirstOrDefault(u => u.UserName == model.Email);
                if (user != null && user.HouseholdId != null)
                {
                    ViewBag.HouseholdId = model.HouseholdId;
                    TempData["error"] = "This user cannot be added to your household.";
                    return View(model);
                }

                var invite = new Invitation(model.Email, (int)model.HouseholdId);
               
                db.Invitations.Add(invite);
                db.SaveChanges();

                //var user = db.Users.Find(User.Identity.GetUserId());
                string code = invite.Code;

                var callbackUrl = Url.Action("InviteConfirmation", "Invitations", new { Email = model.Email, Code = code, InviteId = invite.Id }, protocol: Request.Url.Scheme);
                var message = new IdentityMessage();
                message.Destination = model.Email;
                message.Subject = "MMS Invitation";
                message.Body = "Please join my household in MMS by clicking <a href=\"" + callbackUrl + "\">here</a>";
                var emailSvc = new EmailService();
                await emailSvc.SendAsync(message);
                return RedirectToAction("InviteSent", "Invitations");
            }

            // If we got this far, something failed, redisplay form
            ViewBag.HouseholdId = model.HouseholdId;
            return View(model);
        }

        [NoDirectAccess]
        public ActionResult InviteSent()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult InviteConfirmation([Bind(Include = "Email,Code,InviteId")]InviteConfirmationViewModel model)
        {
            var invite = db.Invitations.FirstOrDefault(i => i.Id == model.InviteId);
            var user = db.Users.FirstOrDefault(u => u.UserName == model.Email);
            var household = new Household();

            if (invite == null)
            {
                ViewBag.Household = "There was a problem with the invitation. Please ask for a new one.";
                return View();
            }

            

            household = db.Households.FirstOrDefault(h => h.Id == invite.HouseholdId);

            if (invite != null && invite.Code == model.Code && DateTimeOffset.Now <= invite.Expires)
            {
                if (user != null && user.HouseholdId == null)
                {
                    user.HouseholdId = household.Id;
                    household.Members.Add(user);
                    foreach(var role in usersHelper.ListUserRoles(user.Id))
                    {
                        usersHelper.RemoveFromRole(user.Id, role);
                    }
                    usersHelper.AddToRole(user.Id, "Member");
                    
                    //db.Entry(household).State = EntityState.Modified;
                    //db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Household = "You've been added to household: " + db.Households.FirstOrDefault(h => h.Id == invite.HouseholdId).Name;
                    return View();


                }
                if (user != null && user.HouseholdId != null)
                {
                    ViewBag.Household = "You already belong to household. \nYou must leave one before joining another.";
                    return View();
                }

                if (user == null)
                {
                    
                    return RedirectToAction("AddUserToHousehold", "Account", new { email = model.Email, hId = invite.HouseholdId, backto = "" });
                    //return View();
                }
            }

            else
            {
                ViewBag.Household = "There was a problem with the invitation. Please ask for a new one.";

            }

            return View();
        }

  

        #endregion

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
