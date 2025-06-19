using BTL.Data;
using BTL.Helpers;
using BTL.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BTL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    //[AutoValidateAntiforgeryToken]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CartAPIController : ControllerBase
    {
        private readonly Hshop2023Context _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartAPIController(
            Hshop2023Context context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public ActionResult<ApiResponse<CartModel>> GetCart()
        {
            try
            {
                var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
                var cartModel = new CartModel
                {
                    Items = cartItems,
                    Quantity = cartItems.Sum(p => p.SoLuong),
                    Total = cartItems.Sum(p => p.ThanhTien)
                };
                return Ok(new ApiResponse<CartModel>(cartModel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CartModel>>> AddToCart([FromBody] CartItemRequest item)
        {
            try
            {
                var product = await _context.HangHoas.FindAsync(item.MaHh);
                if (product == null)
                    return NotFound(new ApiResponse("Product not found"));

                var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
                var existingItem = cartItems.FirstOrDefault(p => p.MaHh == item.MaHh);

                if (existingItem != null)
                {
                    existingItem.SoLuong += item.Quantity;
                }
                else
                {
                    cartItems.Add(new CartItem
                    {
                        MaHh = product.MaHh,
                        TenHH = product.TenHh,
                        DonGia = product.DonGia ?? 0,
                        Hinh = product.Hinh ?? string.Empty,
                        SoLuong = item.Quantity
                    });
                }

                HttpContext.Session.Set(MySetting.CART_KEY, cartItems);

                return Ok(new ApiResponse<CartModel>(new CartModel
                {
                    Items = cartItems,
                    Quantity = cartItems.Sum(p => p.SoLuong),
                    Total = cartItems.Sum(p => p.ThanhTien)
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("{maHh}")]
        public ActionResult<ApiResponse<CartModel>> RemoveFromCart(int maHh)
        {
            try
            {
                var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY);
                if (cartItems == null)
                    return BadRequest(new ApiResponse("Cart not found"));

                var itemToRemove = cartItems.FirstOrDefault(p => p.MaHh == maHh);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                    HttpContext.Session.Set(MySetting.CART_KEY, cartItems);
                }

                return Ok(new ApiResponse<CartModel>(new CartModel
                {
                    Items = cartItems,
                    Quantity = cartItems.Sum(p => p.SoLuong),
                    Total = cartItems.Sum(p => p.ThanhTien)
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse($"Internal server error: {ex.Message}"));
            }
        }
    }

    public class CartItemRequest
    {
        public int MaHh { get; set; }
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
    }
}
