using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Thoughtwave.Models
{
    public class Thought
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

        public string Slug
        {
            get
            {
                string str = this.Title;

                // invalid chars           
                str = Regex.Replace(str, @"[^a-zA-Z0-9 -]", "");

                // convert multiple spaces into one space   
                str = Regex.Replace(str, @"\s+", " ").Trim();

                // cut and trim 
                str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();

                // add hyphens
                str = Regex.Replace(str, @"\s", "-");    
                return str;
            }
        }
        
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual User Author { get; set; }
    }
}