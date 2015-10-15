using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebRoleAds;
using System.Web.Security;
using WorkerRoleAds.Core;

namespace WebRoleAds.Controllers
{
    public class TbPostsController : Controller
    {
        private contosoadsg4Entities db = new contosoadsg4Entities();



        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username, Password")] TbUser tbUser)
        {
            List<TbUser> lstUser = db.TbUsers.ToList();
            for (int i = 0; i < lstUser.Count; i++)
            {
                if (lstUser[i].Username.Equals(tbUser.Username) && lstUser[i].Password.Equals(tbUser.Password))
                {

                    Session["user"] = lstUser[i].Email;
                    return RedirectToAction("Index");
                }
            }

                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return RedirectToAction("Login");
        }

        // GET: TbPosts
        public ActionResult Index()
        {
            return View(db.TbPosts.ToList());
        }

        // GET: TbPosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TbPost tbPost = db.TbPosts.Find(id);
            if (tbPost == null)
            {
                return HttpNotFound();
            }
            return View(tbPost);
        }

        // GET: TbPosts/Create
        public ActionResult Create()
        {
            return View();
            
        }

        // POST: TbPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Title,Content,CreateDate,ModifyDate")] TbPost tbPost)
        {
            
            if (ModelState.IsValid)
            {
                tbPost.UserId = 1;
                tbPost.CreateDate = DateTime.Now;
                tbPost.ModifyDate = DateTime.Now;
                db.TbPosts.Add(tbPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbPost);
        }

        // GET: TbPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TbPost tbPost = db.TbPosts.Find(id);
            if (tbPost == null)
            {
                return HttpNotFound();
            }
            return View(tbPost);
        }

        // POST: TbPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Title,Content,CreateDate,ModifyDate")] TbPost tbPost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbPost);
        }

        // GET: TbPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TbPost tbPost = db.TbPosts.Find(id);
            if (tbPost == null)
            {
                return HttpNotFound();
            }
            return View(tbPost);
        }

        // GET: TbPosts/Subcribe/5
        public ActionResult Subcribe()
        {
            EmailManager.SendSimpleMessage(Session["user"] as String, "Somebody", "Hello từ Bản tin mẹo vặt nhà bếp", "Cam.");
            System.Diagnostics.Trace.WriteLine(Session["user"] as String);
            return RedirectToAction("Create");
           // return View(db.TbPosts.ToList());
        }

        // POST: TbPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TbPost tbPost = db.TbPosts.Find(id);
            db.TbPosts.Remove(tbPost);
            db.SaveChanges();
            return RedirectToAction("Index");
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
