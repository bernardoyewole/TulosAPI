using DAL;
using Entities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace TulosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IGenericRepository<ApplicationUser> _genericUserRepository;

        public UserController(IGenericRepository<ApplicationUser> genericUserRepository)
        {
            _genericUserRepository = genericUserRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _genericUserRepository.GetAll();

            return Ok(users);
        }

        [HttpGet]
        [Route("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var users = await _genericUserRepository.GetAll();

            var user = users.Where(x => x.Email == email).FirstOrDefault();

            if (user == null)
            {

                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut]
        [Route("{email}")]
        public async Task<IActionResult> UpdateUser(string email, [FromBody] ApplicationUser updatedUser)
        {
            // Fetch the user by email
            var users = await _genericUserRepository.GetAll();
            var user = users.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update the user information with the new details from the request body
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email; // Optional if email can be changed
            // user.DateOfBirth = updatedUser.DateOfBirth; // Assuming you have a DateOfBirth field

            // Save the updated user information back to the repository
            var result = _genericUserRepository.Update(user);

            if (!result.IsCompleted)
            {
                return StatusCode(500, "An error occurred while updating the user.");
            }

            return Ok("User information updated successfully.");
        }

    }
}
