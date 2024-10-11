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
                List<CartItem> emptyCart = new List<CartItem>();
                return Ok(emptyCart);
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

            // If the item already exists, update the quantity instead
            var existingCartItems = await _genericCartRepository.GetAll();
            var existingItem = existingCartItems
                .FirstOrDefault(ci => ci.UserEmail == cartItem.UserEmail && ci.HmProductId == cartItem.HmProductId && ci.Size == cartItem.Size && ci.Color == cartItem.Color);

            if (existingItem != null)
            {
                // Increase the quantity if the item already exists
                existingItem.Quantity += 1;
                await _genericCartRepository.Update(existingItem);
                return Ok("Cart item updated with new quantity.");
            }

            // Ensure that the Quantity is set to 1 if it's a new item or if quantity is not provided
            cartItem.Quantity = cartItem.Quantity > 0 ? cartItem.Quantity : 1;

            // Add the new cart item with quantity set to 1
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

        // Remove item or reduce quantity from cart
        [HttpDelete("removeFromCart/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var existingCartItem = await _genericCartRepository.GetById(id);
            if (existingCartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            // If item exists, reduce the quantity
            if (existingCartItem.Quantity > 1)
            {
                existingCartItem.Quantity -= 1;

                await _genericCartRepository.Update(existingCartItem);

                return Ok("Cart item quantity reduced by 1.");
            }
            else
            {
                // If the quantity is 1 or less, remove the item from the cart
                var isDeleted = await _genericCartRepository.Delete(id);
                if (isDeleted)
                {
                    return Ok("Cart item removed successfully.");
                }

                return StatusCode(500, "Error removing cart item.");
            }
        }

        [HttpDelete("clearCart/{email}")]
        public async Task<IActionResult> ClearCart(string email)
        {
            // Retrieve all cart items for the specified user
            var cartItems = await _genericCartRepository.GetAll();
            var userCartItems = cartItems.Where(item => item.UserEmail == email).ToList();

            if (userCartItems.Count == 0)
            {
                return NotFound("No cart items found for this user.");
            }

            // Iterate over the user's cart items and delete each one
            foreach (var cartItem in userCartItems)
            {
                var isDeleted = await _genericCartRepository.Delete(cartItem.Id);
                if (!isDeleted)
                {
                    return StatusCode(500, "Error occurred while clearing the cart.");
                }
            }

            return Ok("All cart items removed successfully.");
        }
    }
}
