using DAL;
using Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TulosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : Controller
    {
        private IGenericRepository<Favorite> _genericFavoriteRepository;
        private IGenericRepository<ApplicationUser> _genericUserRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(IGenericRepository<Favorite> genericFavoriteRepository, IGenericRepository<ApplicationUser> genericUserRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _genericFavoriteRepository = genericFavoriteRepository;
            _genericUserRepository = genericUserRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var favorites = await _genericFavoriteRepository.GetAll();

            return Ok(favorites);
        }

        [HttpGet]
        [Route("{email}")]
        public async Task<IActionResult> GetFavoritesByUser(string email)
        {
            var users = await _genericUserRepository.GetAll();
            var user = users.Where(user => user.Email == email).FirstOrDefault();

            if (user  == null)
            {
                return BadRequest("User does not exist with this email");
            }

            var favorites = await _genericFavoriteRepository.GetAll();
            var favoritesByUser = favorites.Where(f => f.UserEmail == user.Email);

            return Ok(favoritesByUser);
        }

        [HttpPost("addToFavorite")]
        public async Task<IActionResult> AddFavorite(Favorite favorite)
        {
            if (favorite != null)
            {
                // If favorite exists, remove from list
                var allFavorites = await _genericFavoriteRepository.GetAll();
                if (allFavorites.Any(fav => fav.HmProductId == favorite.HmProductId))
                {
                    var existingFavorite = allFavorites.Where(fav => fav.HmProductId == favorite.HmProductId).FirstOrDefault();
                    var isDeleted = await _genericFavoriteRepository.Delete(existingFavorite.Id);

                    if (isDeleted)
                    {
                        return Ok("Favorite removed");
                    }
                }

                // If favorite does not exist, add to list
                await _genericFavoriteRepository.Add(favorite);
                return Ok("Product added to favorites");
            } else
            {
                return BadRequest("Favorite is null");
            }
        }
    }
}
