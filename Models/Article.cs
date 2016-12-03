using System;
using System.Collections.Generic;

namespace Thoughtwave.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public Category Category { get; set; }

        public string Lead 
        { 
            get 
            {
                return Content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)[0];
            }
        }
        
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual User Author { get; set; }
    }
}