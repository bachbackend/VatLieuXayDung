using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VatLieuXayDung.DataAccess;
using VatLieuXayDung.DTO;
using VatLieuXayDung.Extensions;
using VatLieuXayDung.Service;

namespace VatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public UserController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDTO registerDTO)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == registerDTO.Username);

            if (existingUser != null)
                return BadRequest(new { message = "Username is already exists" });


            // Tạo người dùng nhưng chưa kích hoạt
            User user = new User
            {
                Username = registerDTO.Username,
                Password = registerDTO.Password.Hash(),
                Role = 1,
                Status = 1,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Create admin account successfully." });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == registerDTO.Username);

            if (existingUser != null)
                return BadRequest(new { message = "Username is already exists" });


            // Tạo người dùng nhưng chưa kích hoạt
            User user = new User
            {
                Username = registerDTO.Username,
                Password = registerDTO.Password.Hash(),
                Role = 2,
                Status = 1,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Create account successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == loginDTO.Username);

            if (user == null)
                return BadRequest(new { message = "Sai tên đăng nhập." });

            // Kiểm tra mật khẩu (so sánh đã băm)
            if (!loginDTO.Password.Verify(user.Password))
                return BadRequest(new { message = "Sai mật khẩu." });


            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Tạo JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            // Cập nhật Issuer và Audience trong SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token hết hạn sau 1 giờ
                Issuer = _configuration["Jwt:Issuer"],  // Thêm Issuer
                Audience = _configuration["Jwt:Audience"],  // Thêm Audience
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Trả về JWT token
            return Ok(new
            {
                token = tokenString,
                userId = user.Id,
                username = user.Username,
                role = user.Role
            });
        }

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.NewPassword) || string.IsNullOrEmpty(dto.OldPassword))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            if (dto.NewPassword == dto.OldPassword)
            {
                return BadRequest(new { message = "New password cannot equal with old password." });
            }

            var user = await _context.Users
                .Where(a => a.Id == dto.UserId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (!dto.OldPassword.Verify(user.Password))
            {
                return BadRequest(new { message = "Wrong password." });
            }

            user.Password = dto.NewPassword.Hash();
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully." });
        }


    }
}
