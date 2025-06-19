using AutoMapper;
using BTL.Data;
using BTL.Helpers;
using BTL.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class KhachHangAPIController : ControllerBase
    {
        private readonly Hshop2023Context _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<KhachHang> _passwordHasher;

        public KhachHangAPIController(Hshop2023Context context, IMapper mapper, IPasswordHasher<KhachHang> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.KhachHangs.AnyAsync(kh => kh.MaKh == model.MaKh || kh.Email == model.Email))
            {
                ModelState.AddModelError("MaKh", "Tên đăng nhập hoặc Email đã tồn tại.");
                return BadRequest(ModelState);
            }

            var khachHang = _mapper.Map<KhachHang>(model);

            // Generate RandomKey and Hash Password
            khachHang.RandomKey = MyUtil.GenerateRamdomKey();
            khachHang.MatKhau = _passwordHasher.HashPassword(khachHang, model.MatKhau); ;
            khachHang.HieuLuc = true;
            khachHang.VaiTro = 0;

            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            // Optionally return the created user details (without password)
            var registeredUser = _mapper.Map<KhachHangResponseDto>(khachHang);
            return CreatedAtAction(nameof(GetKhachHang), new { id = registeredUser.MaKh }, new ApiResponse<KhachHangResponseDto>(registeredUser));

            // Return success response
            //return StatusCode(201); // Created
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.MaKh == model.UserName || kh.Email == model.UserName);

            if (khachHang == null)
            {
                return Unauthorized(new ApiResponse("Invalid username or password."));
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(khachHang, khachHang.MatKhau, model.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new ApiResponse("Invalid username or password."));
            }

            // Authentication successful, return user info (without sensitive data)
            var loggedInUser = _mapper.Map<KhachHangResponseDto>(khachHang);
            return Ok(new ApiResponse<KhachHangResponseDto>(loggedInUser));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKhachHang(string id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);

            if (khachHang == null)
            {
                return NotFound(new ApiResponse("Customer not found."));
            }

            var khachHangDto = _mapper.Map<KhachHangResponseDto>(khachHang);
            return Ok(new ApiResponse<KhachHangResponseDto>(khachHangDto));
        }
    }
}
