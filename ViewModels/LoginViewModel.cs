using System.ComponentModel.DataAnnotations;

namespace OnlineFoodOrderingSystem.ViewModels
{
    // View model for the login form.
    // Contains only the necessary fields for user authentication.
    // We use view models instead of database models to control what data is sent/received from forms
    public class LoginViewModel
    {
        // User's email address (also used as username)
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // User's password
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // Whether to create a persistent authentication cookie
        // If true, user stays logged in even after closing browser
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}