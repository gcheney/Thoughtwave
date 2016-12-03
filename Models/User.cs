using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Thoughtwave.Models
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime SignUpDate { get; set; }
        public string Avatar { get; set; }
        public string Bio { get; set; }

        public string FullName 
        { 
            get 
            {
                return this.FirstName + " " + this.LastName;
            }
        }
        
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
