using AutoMapper;
using BTL.Data;
using BTL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json.Serialization;

namespace BTL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class HangHoasAPIController : ControllerBase
    {
        private readonly Hshop2023Context _context;
        private readonly IMapper _mapper;

        public HangHoasAPIController(Hshop2023Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/HangHoasAPI
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ApiPagedResponse<HangHoa>>>> GetHangHoas([FromQuery] ProductSearchRequest request)
        {
            var totalCount = await _context.HangHoas.CountAsync();
            var hangHoas = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaNccNavigation)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return Ok(new ApiResponse<ApiPagedResponse<HangHoa>>(
                    new ApiPagedResponse<HangHoa>(
                        hangHoas,
                        totalCount,
                        request.Page,
                        request.PageSize
                    )));
            //return await _context.HangHoas.ToListAsync();
        }

        // GET: api/HangHoasAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<HangHoa>>> GetHangHoa(int id)
        {
            var hangHoa = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaNccNavigation)
                .FirstOrDefaultAsync(h => h.MaHh == id);

            if (hangHoa == null)
            {
                return NotFound();
            }

            return Ok(new ApiResponse<HangHoa>(hangHoa)); // Wrap the HangHoa in ApiResponse
        }

        // PUT: api/HangHoasAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHangHoa(int id, HangHoa hangHoa)
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                System.Diagnostics.Debug.WriteLine($"Raw Request Body: {body}");
            }
            if (id != hangHoa.MaHh)
            {
                return BadRequest();
            }

            _context.Entry(hangHoa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HangHoaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/HangHoasAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HangHoa>> PostHangHoa(HangHoa hangHoa)
        {
            _context.HangHoas.Add(hangHoa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHangHoa", new { id = hangHoa.MaHh }, hangHoa);
        }

        // DELETE: api/HangHoasAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHangHoa(int id)
        {
            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            _context.HangHoas.Remove(hangHoa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHh == id);
        }
    }
}
