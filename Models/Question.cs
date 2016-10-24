using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sophophile.Models
{
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QuestionId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Solved { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}