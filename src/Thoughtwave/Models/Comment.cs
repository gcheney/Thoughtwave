using System;

namespace Thoughtwave.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }

        public int ThoughtId { get; set; }
        public virtual Thought Thought { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}