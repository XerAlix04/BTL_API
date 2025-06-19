using BTL.Data;
using Microsoft.AspNetCore.Mvc;
using BTL.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BTL.Services;

namespace BTL.Controllers
{
    public class HangHoaController : Controller
    {
        //private readonly Hshop2023Context db;

        //public HangHoaController(Hshop2023Context context)
        //{
        //    db = context;
        //}

        //public IActionResult Index(int? loai)
        //{
        //    var hangHoas = db.HangHoas.AsQueryable(); // Truy vấn ban đầu

        //    if (loai.HasValue)
        //    {
        //        hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value); // Gán lại biến khi lọc
        //    }

        //    var items = hangHoas.Select(hh => new HangHoaVM
        //    {
        //        MaHh = hh.MaHh,
        //        TenHh = hh.TenHh,
        //        Gia = hh.DonGia ?? 0,
        //        Hinh = hh.Hinh ?? "",
        //        MoTaNgan = hh.MoTaDonVi ?? "",
        //        TenLoai = hh.MaLoaiNavigation.TenLoai
        //    }).ToList(); // Chuyển về danh sách

        //    return View(items);
        //}
        //public IActionResult search(string? query)
        //{
        //    var hangHoas = db.HangHoas.AsQueryable(); // Truy vấn ban đầu

        //    if (query!=null)
        //    {
        //        hangHoas = hangHoas.Where(p => p.TenHh.Contains(query)); // Gán lại biến khi lọc
        //    }

        //    var items = hangHoas.Select(hh => new HangHoaVM
        //    {
        //        MaHh = hh.MaHh,
        //        TenHh = hh.TenHh,
        //        Gia = hh.DonGia ?? 0,
        //        Hinh = hh.Hinh ?? "",
        //        MoTaNgan = hh.MoTaDonVi ?? "",
        //        TenLoai = hh.MaLoaiNavigation.TenLoai
        //    }).ToList(); // Chuyển về danh sách

        //    return View(items);
        //}
        //public IActionResult Detail(int id)
        //{
        //    var data = db.HangHoas
        //        .Include(p => p.MaLoaiNavigation)
        //        .SingleOrDefault(p => p.MaHh == id);
        //    if (data == null)
        //    {
        //        TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
        //        return Redirect("/404");
        //    }

        //    var result = new ChiTietHangHoaVM
        //    {
        //        MaHh = data.MaHh,
        //        TenHH = data.TenHh,
        //        DonGia = data.DonGia ?? 0,
        //        ChiTiet = data.MoTa ?? string.Empty,
        //        Hinh = data.Hinh ?? string.Empty,
        //        MoTaNgan = data.MoTaDonVi ?? string.Empty,
        //        TenLoai = data.MaLoaiNavigation.TenLoai,
        //        SoLuongTon = 10,//tính sau
        //        DiemDanhGia = 5,//check sau
        //    };
        //    return View(result);
        //}
        private readonly ApiService _apiService;

        public HangHoaController(ApiService apiService)
        {
            _apiService = apiService;
        }

        //public IActionResult Index()
        //{
        //    ViewBag.Message = "HangHoa/Index was hit!";
        //    return View("TestSimple"); // Create a new view named TestSimple.cshtml
        //}

        public async Task<IActionResult> Index(ProductSearchRequest request)
        {
            ApiPagedResponse<HangHoaVM> pagedResult = await _apiService.GetProductsAsync(request);
            if (pagedResult != null)
            {
                ViewBag.TotalCount = pagedResult.TotalCount;
                ViewBag.Page = pagedResult.Page;
                ViewBag.PageSize = pagedResult.PageSize;
                
                return View(pagedResult.Items); // Pass the list of products to the view
            }
            else
            {
                // Set default values for ViewBag in case of an error
                ViewBag.TotalCount = 0;
                ViewBag.Page = 1;
                ViewBag.PageSize = request.PageSize; // Or a default page size like 12
                return View(new List<HangHoaVM>()); // Handle potential errors by showing an empty list
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var productDetails = await _apiService.GetProductDetailsAsync(id);
            if (productDetails == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404"); // Use your actual 404 page route
            }
            return View(productDetails); // Your View should now expect ChiTietHangHoaVM
        }

        [HttpGet]
        public async Task<IActionResult> Search(string search, int? maLoai, double? minPrice, double? maxPrice, string sortBy, bool? sortDesc, int page = 1, int pageSize = 12)
        {
            var request = new ProductSearchRequest
            {
                Search = search,
                MaLoai = maLoai,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                SortDesc = sortDesc,
                Page = page,
                PageSize = pageSize
            };
            var pagedResult = await _apiService.GetProductsAsync(request);

            if (pagedResult != null)
            {
                return View("Search", pagedResult.Items); // Return the Search view with the list of products
            }
            else
            {
                return View("Search", new List<HangHoaVM>()); // Return an empty list if there's an error
            }
        }
    }
}
