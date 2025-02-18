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
        private readonly MailService _mailService;

        public UserController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings, MailService mailService)
        {
            _configuration = configuration;
            _context = context;
            _paginationSettings = paginationSettings.Value;
            _mailService = mailService;
        }

        private static readonly char[] Characters =
       "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        private static readonly Random Random = new Random();

        public static string GenerateRandomString(int length = 32)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be greater than 0", nameof(length));
            }

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Characters[Random.Next(Characters.Length)];
            }

            return new string(result);
        }

        [HttpPost("ResetPassword/{email}")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Generate a reset token
            string resetToken = GenerateRandomString();
            DateTime expiry = DateTime.Now.AddHours(1); // Token hết hạn sau 1 giờ

            // Save token and expiry to database
            user.ResetToken = resetToken;
            user.ResetTokenExpired = expiry;
            await _context.SaveChangesAsync();

            // Generate reset link with token
            string resetLink = $"http://127.0.0.1:5500/confirmResetPassword.html?token={resetToken}";

            var subject = "Reset Password";
            var message = $"Click the link below to reset your password:\n{resetLink}\n\nLink will expire in 1 hour.";

            // Send email
            _mailService.SendEmailAsync(user.Email, subject, message);

            // Return result including email and token
            return Ok(new
            {
                message = "Password reset link has been sent to your email.",
                email = user.Email,
                token = resetToken,
                resetLink = resetLink
            });
        }

        //[HttpPost("ConfirmResetPassword")]
        //public async Task<IActionResult> ConfirmResetPassword([FromQuery] string email, [FromQuery] string token, [FromBody] ResetPasswordRequestDTO request)
        //{
        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Email == email && u.ResetTokenExpired > DateTime.UtcNow);

        //    if (user == null)
        //    {
        //        return BadRequest(new { message = "Invalid or expired request." });
        //    }

        //    // Check token validity
        //    if (user.ResetToken != token)
        //    {
        //        return BadRequest(new { message = "Invalid or expired token." });
        //    }

        //    // Hash new password and save to DB
        //    user.Password = request.NewPassword.Hash();
        //    user.ResetToken = null; // Remove token
        //    user.ResetTokenExpired = null; // Remove token expiry
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Mật khẩu đã được đặt lại thành công." });
        //}

        [HttpPost("ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] string token, [FromBody] ResetPasswordRequestDTO request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ResetToken == token && u.ResetTokenExpired > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }

            // Check token validity
            //if (user.ResetToken != token)
            //{
            //    return BadRequest(new { message = "Invalid or expired token." });
            //}

            // Hash new password and save to DB
            user.Password = request.NewPassword.Hash();
            user.ResetToken = null; // Remove token
            user.ResetTokenExpired = null; // Remove token expiry
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mật khẩu đã được đặt lại thành công." });
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
