using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VatLieuXayDung.DataAccess;
using VatLieuXayDung.DTO;
using VatLieuXayDung.Service;

namespace VatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;
        private readonly IWebHostEnvironment _environment;

        public CertificateController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings, IWebHostEnvironment environment)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
            _environment = environment;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllBlogCategory()
        {
            return Ok(_context.Certificates.AsQueryable());
        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllCertificatesPaging(int pageNumber = 1, int? pageSize = null)
        {
            // Use the pageSize provided by the client, or the default from appsettings.json
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;

            var query = _context.Certificates.AsQueryable();

            // Apply paging
            var certificate = await query
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .ToListAsync();
            return Ok(certificate);
        }

        [HttpGet("GetPageCount")]
        public async Task<IActionResult> GetPageCount()
        {
            int certificate = await _context.Certificates.CountAsync();
            int page = (int)MathF.Ceiling((float)certificate / _paginationSettings.DefaultPageSize);
            return Ok(page);
        }

        [HttpPost("AddCertificate")]
        public async Task<IActionResult> AddProduct([FromForm] CertificateRequestDTO model, IFormFile file)
        {
            // Kiểm tra nếu ảnh được upload
            if (file == null || file.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file type.");
            }

            // Lưu ảnh vào thư mục
            var fileName = Guid.NewGuid() + extension;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/certificate", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var certificate = new Certificate
            {
                Name = model.Name, 
                Description = model.Description,
                Image = fileName,  
                CreatedAt = DateTime.UtcNow,
            };

            // Lưu sản phẩm vào database
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return Ok(new { productId = certificate.Id, fileName = certificate.Image });
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditCategory(int id, [FromBody] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound("Category không tồn tại.");
            }

            var duplicateCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);
            if (duplicateCategory != null)
            {
                return BadRequest("Tên Category đã tồn tại.");
            }

            existingCategory.Name = dto.Name;
            existingCategory.Description = dto.Description;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            return Ok(existingCategory);
        }
    }
}
