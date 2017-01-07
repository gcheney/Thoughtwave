using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft​.AspNetCore​.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thoughtwave.Models;
using Thoughtwave.Data;
using Thoughtwave.ViewModels.ManageViewModels;
using Thoughtwave.Services;
using AutoMapper;

namespace Thoughtwave.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private IHostingEnvironment _environment;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IThoughtwaveRepository _repository;

        public ManageController(
            IHostingEnvironment environment,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IThoughtwaveRepository repository)
        {
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _repository = repository;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        [Route("/manage")]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.UpdateProfileSuccess ? "Your profile has been updated."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var model = new IndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
                Logins = await _userManager.GetLoginsAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };
            return View(model);
        }

        [HttpGet]
        [Route("/manage/profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                _logger.LogError("No current user found");
                return View("Error");
            }

            var model = Mapper.Map<EditProfileViewModel>(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                _logger.LogError("No current user found");
                return View("Error");
            }

            // set profile image
            user.Avatar = await SaveProfileImageAsync(HttpContext.Request.Form.Files);

            // update user
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Bio = model.Bio;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.UpdateProfileSuccess });
            }

            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(string username)
        {
            var userToDelete = await _userManager.FindByNameAsync(username);
            var currentUser = await GetCurrentUserAsync();

            if (userToDelete == null || currentUser == null)
            {
                _logger.LogError($"Unable to find user with username {username}");
                return View("Error");
            }

            if (userToDelete.Id == currentUser.Id)
            {
                // sign user out 
                await _signInManager.SignOutAsync();

                // delete user avatar
                DeleteImage(userToDelete.Avatar);

                // delete users thoughts images
                await DeleteThoughtImagesAsync(userToDelete);

                // delete user
                var result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }         
                _logger.LogError($"Unable to delete user {userToDelete.Id}");
                return View("Error");
            }

            return Forbid();
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            ManageMessageId? message = ManageMessageId.Error;
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _smsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(1, "User enabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(2, "User disabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }
            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Failed to verify phone number");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
                }
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes()
                .Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            UpdateProfileSuccess,
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<string> SaveProfileImageAsync(IFormFileCollection files)
        {
            foreach (var image in files)
            {
                if (image != null && image.Length > 0)
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, "dist/uploads/avatars");
                    var filePath = Path.Combine(imagePath, image.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                        return $"/dist/uploads/avatars/{image.FileName}";
                    }
                }
            }

            return null;
        }

        private async Task DeleteThoughtImagesAsync(User user)
        {
            var thoughts = await _repository.GetThoughtsByUserNameAsync(user.UserName);

            if (thoughts == null)
            {
                return;
            }
            foreach (var thought in thoughts)
            {
                DeleteImage(thought.Image);
            }
        }

        private void DeleteImage(string filePath)
        {
            var imagePath = _environment.WebRootPath + filePath;
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        #endregion
    }
}
