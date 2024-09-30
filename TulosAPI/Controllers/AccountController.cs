using Entities.Entities;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TulosAPI.Services;
using IEmailSender = TulosAPI.Services.IEmailSender;

namespace TulosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, TulosAPI.Services.IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Generate email confirmation token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        nameof(ConfirmEmail),
                        "Account",
                        new { userId = user.Id, code },
                        protocol: HttpContext.Request.Scheme);

                    // Create MailData object for the email
                    var mailData = new MailData
                    {
                        ToEmail = model.Email,
                        ToName = model.FirstName + " " + model.LastName,
                        Subject = "Confirm your email",
                        Body = $"Please activate your account by clicking this link -  {callbackUrl}."
                    };

                    // Send confirmation email
                    var emailSent = await _emailSender.SendMailAsync(mailData);

                    if (!emailSent)
                    {
                        // Handle failure in sending email
                        await _userManager.DeleteAsync(user);
                        return StatusCode(500, "Error sending account activation email. Please try again.");
                    }

                    return Ok(new { message = "User registered successfully. Please check your email to confirm your account." });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("User ID and Code are required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok("Account activated successfully.");
            }

            return BadRequest("Error confirming email.");
        }

        [HttpPost("checkUser")]
        public async Task<IActionResult> CheckUser([FromBody] string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Ok("User not found");
            } else
            {
                return Ok("User confirmed successfully");
            }
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return BadRequest(new { message = "If the email exists and is confirmed, a password reset link will be sent." });
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Create a callback URL for password reset with query parameters
            var callbackUrl = $"http://localhost:3000/resetPassword?email={model.Email}&code={resetToken}";

            var mailData = new MailData
            {
                ToEmail = user.Email,
                ToName = $"{user.FirstName} {user.LastName}",
                Subject = "Reset your password",
                Body = $"Please reset your password by clicking this link - {callbackUrl}"
            };

            // Send the password reset email
            var emailSent = await _emailSender.SendMailAsync(mailData);

            if (!emailSent)
            {
                return StatusCode(500, "Error sending password reset email. Please try again.");
            }

            return Ok(new { message = "Password reset email sent. Please check your email." });
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid request." });
            }

            // Check if the new password is the same as the current password
            var isCurrentPassword = await _userManager.CheckPasswordAsync(user, model.NewPassword);
            if (isCurrentPassword)
            {
                return BadRequest(new { message = "New password cannot be the same as the old one." });
            }

            // Reset the user's password using the provided token
            var result = await _userManager.ResetPasswordAsync(user, model.ResetCode, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { message = "Password has been reset successfully." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verify the current password
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordCheck)
            {
                return BadRequest("Current password is incorrect.");
            }

            // Check if new password is same as old password
            var newPasswordCheck = await _userManager.CheckPasswordAsync(user, model.NewPassword);
            if (newPasswordCheck)
            {
                return BadRequest("New password cannot be same as old  password");
            }

            // Change the password
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok("Password changed successfully.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "User logged out successfully" });
        }
    }
}
