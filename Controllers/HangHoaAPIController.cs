using AutoMapper;
using BTL.Data;
using BTL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class HangHoaAPIController : ControllerBase
    {
        private readonly Hshop2023Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HangHoaAPIController> _logger;

        // Constructor that accepts Hshop2023Context for injection
        public HangHoaAPIController(Hshop2023Context context, IMapper mapper, ILogger<HangHoaAPIController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ApiPagedResponse<HangHoaVM>>>> GetProducts(
            [FromQuery] ProductSearchRequest request)
        {
            _logger.LogInformation("GetProducts endpoint was hit.");
            try
            {
                var query = _context.HangHoas
                    .Include(h => h.MaLoaiNavigation)
                    .AsQueryable();

                // Filtering
                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(h => h.TenHh.Contains(request.Search));

                if (request.MaLoai.HasValue)
                    query = query.Where(h => h.MaLoai == request.MaLoai);

                if (request.MinPrice.HasValue)
                    query = query.Where(h => h.DonGia >= request.MinPrice);

                if (request.MaxPrice.HasValue)
                    query = query.Where(h => h.DonGia <= request.MaxPrice);

                // Sorting
                query = request.SortBy?.ToLower() switch
                {
                    "price" => request.SortDesc ?? false
                        ? query.OrderByDescending(h => h.DonGia)
                        : query.OrderBy(h => h.DonGia),
                    "name" => request.SortDesc ?? false
                        ? query.OrderByDescending(h => h.TenHh)
                        : query.OrderBy(h => h.TenHh),
                    _ => query.OrderBy(h => h.MaHh)
                };

                // Pagination
                var totalCount = await query.CountAsync();
                var products = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return Ok(new ApiResponse<ApiPagedResponse<HangHoaVM>>(
                    new ApiPagedResponse<HangHoaVM>(
                        _mapper.Map<List<HangHoaVM>>(products),
                        totalCount,
                        request.Page,
                        request.PageSize
                    )));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products.");
                return StatusCode(500, new ApiResponse($"Internal server error: {ex.Message}"));
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<HangHoaVM>>> GetProduct(int id)
        {
            _logger.LogInformation("GetProduct endpoint was hit.");
            try
            {
                var product = await _context.HangHoas
                    .Include(h => h.MaLoaiNavigation)
                    .FirstOrDefaultAsync(h => h.MaHh == id);

                if (product == null)
                    return NotFound(new ApiResponse("Product not found"));

                return Ok(new ApiResponse<HangHoaVM>(_mapper.Map<HangHoaVM>(product)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product.");
                return StatusCode(500, new ApiResponse($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<ApiResponse<ChiTietHangHoaVM>>> GetProductDetails(int id)
        {
            _logger.LogInformation("GetProductDetails endpoint was hit.");
            try
            {
                var product = await _context.HangHoas
                    .Include(h => h.MaLoaiNavigation)
                    .FirstOrDefaultAsync(h => h.MaHh == id);

                if (product == null)
                    return NotFound(new ApiResponse("Product not found"));

                var result = _mapper.Map<ChiTietHangHoaVM>(product);
                // You might need to fetch SoLuongTon and DiemDanhGia from other sources
                result.SoLuongTon = 10; // Example
                result.DiemDanhGia = 5; // Example

                return Ok(new ApiResponse<ChiTietHangHoaVM>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product details.");
                return StatusCode(500, new ApiResponse($"Internal server error: {ex.Message}"));
            }
        }
    }
    public class ProductSearchRequest
    {
        public string? Search { get; set; }
        public int? MaLoai { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
