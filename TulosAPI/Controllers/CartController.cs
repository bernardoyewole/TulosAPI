using DAL;
using Entities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace TulosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private IGenericRepository<CartItem> _genericCartRepository;
        private IGenericRepository<ApplicationUser> _genericUserRepository;

        public CartController(IGenericRepository<CartItem> genericCartRepository, IGenericRepository<ApplicationUser> genericUserRepository)
        {
            _genericCartRepository = genericCartRepository;
            _genericUserRepository = genericUserRepository;
        }

        // Get all cart items for a specific user
        [HttpGet("{email}")]
        public async Task<IActionResult> GetCartItems(string email)
        {
            var cartItems = await _genericCartRepository.GetAll();
            var userCartItems = cartItems.Where(ci => ci.UserEmail == email).ToList();

            if (userCartItems == null || userCartItems.Count == 0)
            {
                return Ok("No cart items found for this user.");
            }

            return Ok(userCartItems);
        }

        // Add item to cart
        [HttpPost("addToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Invalid cart item.");
            }

            // Check if item already exists in the cart for this user
            var existingCartItems = await _genericCartRepository.GetAll();
            var existingItem = existingCartItems
                .FirstOrDefault(ci => ci.UserEmail == cartItem.UserEmail && ci.HmProductId == cartItem.HmProductId && ci.Size == cartItem.Size && ci.Color == cartItem.Color);

            if (existingItem != null)
            {
                // If the item already exists, update the quantity instead
                existingItem.Quantity += cartItem.Quantity;
                await _genericCartRepository.Update(existingItem);
                return Ok("Cart item updated with new quantity.");
            }

            // Add new cart item
            await _genericCartRepository.Add(cartItem);
            return Ok("Product added to cart.");
        }

        // Update cart item (change quantity)
        [HttpPut("updateCart/{id}")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItem cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Invalid cart item.");
            }

            var existingCartItem = await _genericCartRepository.GetById(cartItem.Id);
            if (existingCartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            existingCartItem.Quantity = cartItem.Quantity;

            await _genericCartRepository.Update(existingCartItem);
            return Ok("Cart item updated successfully.");
        }

        // Remove item from cart
        [HttpDelete("removeFromCart/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var existingCartItem = await _genericCartRepository.GetById(id);
            if (existingCartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            var isDeleted = await _genericCartRepository.Delete(id);
            if (isDeleted)
            {
                return Ok("Cart item removed successfully.");
            }

            return StatusCode(500, "Error removing cart item.");
        }
    }
}
