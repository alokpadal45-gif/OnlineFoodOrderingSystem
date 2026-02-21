using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineFoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.ViewModels;
using System;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Controllers
{
    // Controller responsible for handling user authentication and account management.
    // Includes login, logout, registration, and access denied functionality.
    public class AccountController : Controller
    {
        // Dependency injection of required services
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        // Constructor that injects required Identity services
        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // Displays the login page
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]  // Allow access to non-authenticated users
        public IActionResult Login(string? returnUrl = null)
        {
            // Store return URL for redirect after login
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Processes the login form submission
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]  // Protect against CSRF attacks
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Validate the model
            if (ModelState.IsValid)
            {
                // Attempt to sign in the user
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,           // Username (we use email as username)
                    model.Password,        // Password
                    model.RememberMe,      // Create persistent cookie if true
                    lockoutOnFailure: true // Lock account after multiple failed attempts
                );

                if (result.Succeeded)
                {
                    // Login successful - redirect to return URL or home page
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                if (result.IsLockedOut)
                {
                    // Account is locked due to multiple failed login attempts
                    ModelState.AddModelError(string.Empty, 
                        "Your account has been locked due to multiple failed login attempts. Please try again later.");
                }
                else
                {
                    // Invalid login credentials
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we got this far, something failed - redisplay form
            return View(model);
        }

        // Displays the registration page
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // Processes the registration form submission
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create new user object
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                // Create user with password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // User created successfully - assign Customer role by default
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // Automatically sign in the new user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to home page
                    return RedirectToAction("Index", "Home");
                }

                // Add errors to ModelState if user creation failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed - redisplay form
            return View(model);
        }

        // Logs out the current user
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await _signInManager.SignOutAsync();

            // Redirect to home page
            return RedirectToAction("Index", "Home");
        }

        // Displays the access denied page when user tries to access unauthorized resources
        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}