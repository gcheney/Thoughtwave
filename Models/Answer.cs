using System;

namespace Sophophile.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Question Question { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}