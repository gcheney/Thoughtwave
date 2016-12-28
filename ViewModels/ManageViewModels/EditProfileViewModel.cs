using System.ComponentModel.DataAnnotations;

namespace Thoughtwave.ViewModels.ManageViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name="First Name")]
        [StringLength(50, 
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
            MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name="Last Name")]
        [StringLength(50, 
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
            MinimumLength = 1)]
        public string LastName { get; set; }

        public string Avatar { get; set; }

        [Required]
        [Display(Name="Your Bio/Details")]
        [StringLength(500, 
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
            MinimumLength = 10)]
        public string Bio { get; set; }
    }
}
