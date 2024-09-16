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
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
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
                        Body = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>."
                    };

                    // Send confirmation email
                    var emailSent = await _emailSender.SendMailAsync(mailData);

                    if (!emailSent)
                    {
                        // Handle failure in sending email
                        return StatusCode(500, "Error sending confirmation email. Please try again.");
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
                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Error confirming email.");
        }
    }
}
