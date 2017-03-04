using System;
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
        public bool DisableComments { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual User Author { get; set; }
    }
}