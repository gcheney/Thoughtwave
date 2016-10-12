using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sophophile.Models
{
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string ApplicationUserID { get; set; }
        public string Title { get; set; }
        public int Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Solved { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public ApplicationUser User { get; set; }
    }
}