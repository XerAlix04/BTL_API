using AutoMapper;
using BTL.Data;
using BTL.Helpers;
using BTL.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BTL.Services;
using static NuGet.Packaging.PackagingConstants;

namespace BTL.Controllers
{
    public class KhachHangController : Controller
    {
        //private readonly Hshop2023Context db;
        //private readonly ApiService _apiService;
        //private readonly IMapper _mapper;

        //public KhachHangController(Hshop2023Context context, IMapper mapper)
        //{
        //    db = context;
        //    _mapper = mapper;
        //}

        //[HttpGet]
        //public IActionResult DangKy()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var khachHang = _mapper.Map<KhachHang>(model);
        //            khachHang.RandomKey = MyUtil.GenerateRamdomKey();
        //            khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
        //            khachHang.HieuLuc = true;//sẽ xử lý khi dùng Mail để active
        //            khachHang.VaiTro = 0;

        //            if (Hinh != null)
        //            {
        //                khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
        //            }

        //            db.Add(khachHang);
        //            db.SaveChanges();
        //            return RedirectToAction("Index", "HangHoa");
        //        }
        //        catch (Exception ex)
        //        {
        //            var mess = $"{ex.Message} shh";
        //        }
        //    }
        //    return View();
        //}
        //#region Login
        //[HttpGet]
        //public IActionResult DangNhap(string? ReturnUrl)
        //{
        //    ViewBag.ReturnUrl = ReturnUrl;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        //{
        //    ViewBag.ReturnUrl = ReturnUrl;
        //    if (ModelState.IsValid)
        //    {
        //        var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
        //        if (khachHang == null)
        //        {
        //            ModelState.AddModelError("loi", "Không có khách hàng này");
        //        }
        //        else
        //        {
        //            if (!khachHang.HieuLuc)
        //            {
        //                ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
        //            }
        //            else
        //            {
        //                if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
        //                {
        //                    ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
        //                }
        //                else
        //                {
        //                    var claims = new List<Claim> {
        //                        new Claim(ClaimTypes.Email, khachHang.Email),
        //                        new Claim(ClaimTypes.Name, khachHang.HoTen),
        //                        new Claim("CustomerID", khachHang.MaKh),

        ////claim - role động
        //new Claim(ClaimTypes.Role, "Customer")
        //                    };

        //                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //                    await HttpContext.SignInAsync(claimsPrincipal);

        //                    if (Url.IsLocalUrl(ReturnUrl))
        //                    {
        //                        return Redirect(ReturnUrl);
        //                    }
        //                    else
        //                    {
        //                        return Redirect("/");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return View();
        //}
        //#endregion
        //[Authorize]
        //public IActionResult Profile()
        //{
        //    return View();
        //}

        //[Authorize]
        //public async Task<IActionResult> DangXuat()
        //{
        //    await HttpContext.SignOutAsync();
        //    return Redirect("/");
        //}
        private readonly Hshop2023Context db;
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context, IMapper mapper, ApiService apiService)
        {
            db = context;
            _mapper = mapper;
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DangKy(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                if (Hinh != null)
                {
                    model.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                }
                else
                {
                    model.Hinh = "man.png"; // Set a default image if none is provided
                }

                var apiResponse = await _apiService.CreateAccountAsync(model);

                if (apiResponse?.Success == true)
                {
                    // Registration successful, redirect to login
                    return RedirectToAction("DangNhap");
                }
                else
                {
                    // Registration failed, add error to ModelState
                    ModelState.AddModelError(string.Empty, apiResponse?.Message ?? "Đăng ký không thành công.");
                }
            }
            return View(model);
        }

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl, bool apiLoginSuccess = false)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (apiLoginSuccess)
            {
                // If we're here after a successful API login, we need to
                // re-render the login form, perhaps with a message, or
                // ideally, trigger the POST logic to set the cookie.
                // For simplicity, let's just pass the model to the view.
                return View();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl, bool apiLoginSuccess = false)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (apiLoginSuccess)
            {
                var apiResponse = await _apiService.LoginAsync(model);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var loggedInUser = apiResponse.Data;
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Email, loggedInUser.Email),
                        new Claim(ClaimTypes.Name, loggedInUser.HoTen),
                        new Claim("CustomerID", loggedInUser.MaKh),
                        new Claim(ClaimTypes.Role, "Customer") // Example role
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Add logging here
                    Console.WriteLine($"Attempting to sign in user: {loggedInUser.MaKh}");
                    await HttpContext.SignInAsync(claimsPrincipal);
                    Console.WriteLine($"User {loggedInUser.MaKh} signed in successfully.");

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, apiResponse?.Message ?? "Lỗi khi xác thực lại sau đăng nhập API.");
                    return View(model);
                }
            }

            // Handle the initial POST request to /KhachHang/DangNhap (if JavaScript is disabled or for direct form submissions)
            if (ModelState.IsValid)
            {
                var apiResponse = await _apiService.LoginAsync(model);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    // ... (rest of your original cookie setting logic) ...
                    var loggedInUser = apiResponse.Data;
                    var claims = new List<Claim> { /* ... */ };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    Console.WriteLine($"Attempting to sign in user: {loggedInUser.MaKh} (direct post)");
                    await HttpContext.SignInAsync(claimsPrincipal);
                    Console.WriteLine($"User {loggedInUser.MaKh} signed in successfully (direct post).");

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, apiResponse?.Message ?? "Đăng nhập không thành công.");
                    return View(model);
                }
            }
            return View(model);
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
