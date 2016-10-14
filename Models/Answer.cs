using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sophophile.Models
{
    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public string ApplicationUserID { get; set; }
        public string Response { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Question Question { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}