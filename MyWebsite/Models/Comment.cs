﻿using System;
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
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTimeOffset Created { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTimeOffset? Updated { get; set; }
        public string UpdateReason { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual BlogPosts Post { get; set; }

        //Post limit

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
