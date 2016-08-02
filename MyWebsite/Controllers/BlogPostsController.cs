using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyWebsite.Models;
using System.IO;
using MyWebsites.Models;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;

namespace MyWebsite.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public BlogPosts BlogPosts { get; private set; }
        public string Slug { get; private set; }

        // GET: BlogPosts
        public ActionResult Index(int? page)
        {

            int pageSize = 3;

            // display three blog posts at a time on this page  

            int pageNumber = (page ?? 1); 

            // return View(db.Posts.ToList());
            return View(db.Posts.OrderByDescending(m => m.Created).ToPagedList(pageNumber, pageSize)); //This places the latest post first
        }

        //Search Implementation
        [HttpPost]
        public ActionResult Index(string searchStr, int? page)
        {

            var result = db.Posts.Where(p => p.Body.Contains(searchStr))
             .Union(db.Posts.Where(p => p.Title.Contains(searchStr)))
                    .Union(db.Posts.Where(p => p.Comments.Any(c => c.Body.Contains(searchStr))))
                        .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.DisplayName.Contains(searchStr))))
                             .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.FirstName.Contains(searchStr))))
                                 .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.LastName.Contains(searchStr))))
                                     .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.UserName.Contains(searchStr))))
                                         .Union(db.Posts.Where(p => p.Comments.Any(c => c.Author.Email.Contains(searchStr))))
                                                 .Union(db.Posts.Where(p => p.Comments.Any(c => c.UpdateReason.Contains(searchStr))));


            int pageSize = 3;

            // display three blog posts at a time on this page  

            int pageNumber = (page ?? 1);

            // return View(db.Posts.ToList());
            return View(result.OrderByDescending(m => m.Created).ToPagedList(pageNumber, pageSize));
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(String Slug)
        {
            if ((String.IsNullOrWhiteSpace(Slug)))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPosts blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }
        
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BlogPosts blogPost = db.Posts.Find(id);
        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(blogPost);
        //}

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]   //This has to be directly above to require admin role to create (Authorization/Authentication)
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Updated,Title,Slug,Body,MediaURL,Published")] BlogPosts blogPosts, HttpPostedFileBase image)
        {


            if (image != null && image.ContentLength > 0)

            { //check the file name to make sure its an image                
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }


            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPosts.Title);
                if (string.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPosts);
                }

                if (db.Posts.Any(p => p.Slug == Slug))
                { 
                    ModelState.AddModelError("Title", "The title must be unique.");

                             return View(blogPosts);
                }
                    blogPosts.Slug = Slug;

                        blogPosts.Created = DateTimeOffset.Now;
                


                    if (image != null)
                    {     //relative server path        
                        var filePath = "/Uploads/";
                        // path on physical drive on server                      
                        var absPath = Server.MapPath("~" + filePath);
                        blogPosts.MediaURL = filePath + image.FileName;
                        //save image                     
                        image.SaveAs(Path.Combine(absPath, image.FileName));
                    }
                    
                    db.Posts.Add(blogPosts);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                

            }
                return View(BlogPosts);
        }
    
    

       


        //private new ActionResult View(object blogPost)
        // {
        //     throw new NotImplementedException();
        // }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")] //This has to be directly above to require admin role to create (Authorization/Authentication)
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPosts blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,Slug,Body,MediaURL,Published")] BlogPosts blogPost)
        {
            if (ModelState.IsValid)
            {
                blogPost.Updated = DateTimeOffset.Now;

                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPosts blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPosts blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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
