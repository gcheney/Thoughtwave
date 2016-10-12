using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Sophophile.Models
{
    // Add profile data for application users by adding properties to the User class
    public class ApplicationUser : IdentityUser
    {
         public ICollection<Question> Questions { get; set; }
         public ICollection<Answer> Answers { get; set; }
    }
}
