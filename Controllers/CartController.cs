using BTL.Data;
using BTL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using BTL.Helpers;
using BTL.Data;
using BTL.Helpers;
using BTL.ViewModel;
using BTL.Services;
using Microsoft.AspNetCore.Authorization;

namespace BTL.Controllers
{
    public class CartController : Controller
    {
        //private readonly Hshop2023Context db;

        //public CartController(Hshop2023Context context)
        //{
        //    db = context;
        //}

        //public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        //public IActionResult Index()
        //{
        //    return View(Cart);
        //}

        //public IActionResult AddToCart(int id, int quantity = 1)
        //{
        //    var gioHang = Cart;
        //    var item = gioHang.SingleOrDefault(p => p.MaHh == id);
        //    if (item == null)
        //    {
        //        var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
        //        if (hangHoa == null)
        //        {
        //            TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
        //            return Redirect("/404");
        //        }
        //        item = new CartItem
        //        {
        //            MaHh = hangHoa.MaHh,
        //            TenHH = hangHoa.TenHh,
        //            DonGia = hangHoa.DonGia ?? 0,
        //            Hinh = hangHoa.Hinh ?? string.Empty,
        //            SoLuong = quantity
        //        };
        //        gioHang.Add(item);
        //    }
        //    else
        //    {
        //        item.SoLuong += quantity;
        //    }

        //    HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

        //    return RedirectToAction("Index");
        //}

        //public IActionResult RemoveCart(int id)
        //{
        //    var gioHang = Cart;
        //    var item = gioHang.SingleOrDefault(p => p.MaHh == id);
        //    if (item != null)
        //    {
        //        gioHang.Remove(item);
        //        HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
        //    }
        //    return RedirectToAction("Index");
        //}
        private readonly ApiService _apiService;
        private readonly Hshop2023Context _db; // You might still need this for fetching product details initially

        public CartController(ApiService apiService, Hshop2023Context db)
        {
            _apiService = apiService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var cartModel = await _apiService.GetCartAsync();
            return View(cartModel?.Items ?? new List<CartItem>());
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var cartItemRequest = new CartItemRequest { MaHh = id, Quantity = quantity };
            var updatedCart = await _apiService.AddToCartAsync(cartItemRequest);

            if (updatedCart != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Handle error - perhaps fetch product details to display an error message
                var hangHoa = await _db.HangHoas.FindAsync(id);
                TempData["Message"] = $"Không thể thêm sản phẩm '{hangHoa?.TenHh}' vào giỏ hàng.";
                return RedirectToAction("Index"); // Or a dedicated error page
            }
        }

        [Authorize]
        public async Task<IActionResult> RemoveCart(int id)
        {
            var updatedCart = await _apiService.RemoveFromCartAsync(id);

            if (updatedCart != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = $"Không thể xóa sản phẩm có mã {id} khỏi giỏ hàng.";
                return RedirectToAction("Index"); // Or a dedicated error page
            }
        }

        public string GetLoggedInCustomerId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.Claims.FirstOrDefault(c => c.Type == "CustomerID")?.Value;
            }
            return null;
        }
    }
}