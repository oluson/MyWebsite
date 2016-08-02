using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyWebsite.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        [AllowHtml]
        public string Body { get; set; }

        public DateTimeOffset Created { get; set; }
       
        public DateTimeOffset? Updated { get; set; }
        public string UpdateReason { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual BlogPosts Post { get; set; }
    
} 
    }
