﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebsite.Models
{
    public class BlogPosts
    {
        public BlogPosts()
        {
            this.Comments = new HashSet<Comment>();
    }
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTimeOffset Created { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTimeOffset? Updated { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string MediaURL { get; set; }
        public bool Published { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }



        //Post limit (call this method in the view)

        private int BodyLimit = 200;
        public string BodyTrimmed
        {
            get
            {
                if (this.Body.Length > this.BodyLimit)
                    return this.Body.Substring(0, this.BodyLimit) + " " + "...";
                else
                    return this.Body;
            }
        }
    }
}