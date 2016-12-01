using System;

namespace Thoughtwave.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Article Article { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}