using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using SendGrid;
using Microsoft.AspNet.Identity;
using static MyWebsite.Models.BlogPosts;
using MyWebsite.Models;

namespace MyWebsite.Controllers
{               [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact([Bind(Include = "Name,Email,Message")] Contact contact)
        {
            //contact.Created = DateTime.Now;
            var newContact = contact.Name;

            var svc = new EmailService();
            var msg = new IdentityMessage();
            msg.Subject = "Contact From Portfolio Site";
            msg.Body = "\r\n You have recieved a request to contact from " + newContact + "(" + contact.Email + ")" + "\r\n"
                         + "\r\n With the following message: \r\n\t"
                         + contact.Message;


            await svc.SendAsync(msg);

            return View(contact);
        }
    

        public ActionResult Resume()
        {
            return View();
        }
        public ActionResult Portfolio()
        {
            return View();
        }
        public ActionResult Blog()
        {
            return View();
        }
    }
}
    