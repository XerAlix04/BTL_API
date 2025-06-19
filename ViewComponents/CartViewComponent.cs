using BTL.Helpers;
using BTL.Services;
using BTL.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BTL.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        //public IViewComponentResult Invoke()
        //{
        //    var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        //    return View("CartPanel", new CartModel
        //    {
        //        Items = cartItems,
        //        Quantity = cartItems.Sum(p => p.SoLuong),
        //        Total = cartItems.Sum(p => p.ThanhTien)
        //    });
        //}
        private readonly ApiService _apiService;

        public CartViewComponent(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartModel = await _apiService.GetCartAsync();
            return View("CartPanel", cartModel ?? new CartModel());
        }
    }
}
