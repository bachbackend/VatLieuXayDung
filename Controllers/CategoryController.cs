using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VatLieuXayDung.Service;
using Microsoft.EntityFrameworkCore;
using VatLieuXayDung.DTO;
using VatLieuXayDung.DataAccess;

namespace VatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public CategoryController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings, IWebHostEnvironment environment)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllBlogCategory()
        {
            return Ok(_context.Categories.AsQueryable());

        }

        [HttpGet("GetAllPaging")]
        public async Task<IActionResult> GetAllBlogCategoryPaging(int pageNumber = 1, int? pageSize = null)
        {
            // Use the pageSize provided by the client, or the default from appsettings.json
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;

            var query = _context.Categories.AsQueryable();

            // Apply paging
            var cate = await query
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .ToListAsync();
            return Ok(cate);
        }

        [HttpGet("GetPageCount")]
        public async Task<IActionResult> GetPageCount()
        {
            int blogCateCount = await _context.Categories.CountAsync();
            int page = (int)MathF.Ceiling((float)blogCateCount / _paginationSettings.DefaultPageSize);
            return Ok(page);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddBlogCategory([FromBody] CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == dto.Name);

            if (existingCategory != null)
            {
                return BadRequest("Category này đã tồn tại.");
            }

            var newCategory = new Category
            {
                Name = dto.Name,
                CreatedAt = DateTime.Now,
                Description = dto.Description
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.Id }, newCategory);
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
