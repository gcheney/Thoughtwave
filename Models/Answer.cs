using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sophophile.Models
{
    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AnswerId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Question Question { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}