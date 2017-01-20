using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Thoughtwave.ExtensionMethods;
using Thoughtwave.Models;

namespace Thoughtwave.ViewModels.ThoughtViewModels
{
    public class ThoughtViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public Category Category { get; set; }
        public bool DisableComments { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }

        public string Lead 
        { 
            get 
            {
                return Content.Length < 500 ? Content : Content.TruncateString(500) + "...";
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

        public List<string> TagList
        {
            get
            {
                return Tags != null ? Tags.Split(',').ToList() : new List<string>();
            }
        }
        
        public ICollection<Comment> Comments { get; set; }
        public User Author { get; set; }
    }
}