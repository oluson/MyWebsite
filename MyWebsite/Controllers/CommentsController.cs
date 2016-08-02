using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyWebsite.Models;
using Microsoft.AspNet.Identity;

namespace MyWebsite.Controllers
{
                [RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        //public ActionResult Index()
        //{
        //    var comments = db.Comments.Include(c => c.Author).Include(c => c.Post);
          //return View(comments.ToList());
        //}

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,PostId,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Author = db.Users.Find(User.Identity.GetUserId());
                comment.AuthorId = User.Identity.GetUserId();
                comment.Body = Request.Params["comments"];
                comment.Created = DateTimeOffset.Now;
                comment.PostId = Convert.ToInt32(Request.Params["postId"]);
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "BlogPosts", routeValues: new { slug = Request.Params["slug"] });
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize] // This has to be directly above to require admin/Moderator role to edit (Authorization/Authentication)
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,Body,Created,UpdateReason,AuthorId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Updated = DateTimeOffset.Now;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                BlogPosts blogPost = db.Posts.FirstOrDefault(p => p.Id == comment.PostId);
                if (blogPost == null)
                {
                    return HttpNotFound();
                }
                return RedirectToAction("Details", "BlogPosts", routeValues: new { slug = blogPost.Slug });
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.PostId = new SelectList(db.Posts, "Id", "Title", comment.PostId);
            return View();
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin,Moderator")] //This has to be directly above to require admin/Moderator role to delete (Authorization/Authentication)
        public ActionResult Delete(int? id, int postId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int postId)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            BlogPosts blogPost = db.Posts.FirstOrDefault(p => p.Id == postId);
            return RedirectToAction("Details", "BlogPosts", routeValues: new { slug = blogPost.Slug });
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
