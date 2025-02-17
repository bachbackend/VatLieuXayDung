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
    public class ArticleCategoryController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public ArticleCategoryController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }
            [HttpGet("GetAll")]
            public async Task<IActionResult> GetAllArticleCategory()
            {
                var ac = _context.ArticleCategories.ToList();
                if (ac == null)
                {
                    return NotFound("Không tìm thấy danh mục nào.");
                }
                return Ok(ac);
            }

            [HttpPost("Add")]
            public async Task<IActionResult> AddAC([FromBody] AcRequest dto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ac = await _context.ArticleCategories
                    .FirstOrDefaultAsync(c => c.Name == dto.Name);

                if (ac != null)
                {
                    return BadRequest("Danh mục này đã tồn tại.");
                }

                var newAc = new ArticleCategory
                {
                    Name = dto.Name,
                };

                _context.ArticleCategories.Add(newAc);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetACById), new { id = newAc.Id }, newAc);
            }

            [HttpGet("GetById/{id}")]
            public async Task<IActionResult> GetACById(int id)
            {
                var ac = await _context.ArticleCategories.FindAsync(id);
                if (ac == null)
                {
                    return NotFound("Danh mục không tồn tại.");
                }
                return Ok(ac);
            }

            [HttpPut("Edit/{id}")]
            public async Task<IActionResult> EditAC(int id, [FromBody] AcRequest dto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingAc = await _context.ArticleCategories.FindAsync(id);
                if (existingAc == null)
                {
                    return NotFound("Danh mục không tồn tại.");
                }

                var duplicateAC = await _context.ArticleCategories
                    .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);
                if (duplicateAC != null)
                {
                    return BadRequest("Tên danh mục đã tồn tại.");
                }

                existingAc.Name = dto.Name;

                _context.ArticleCategories.Update(existingAc);
                await _context.SaveChangesAsync();

                return Ok(existingAc);
            }
    }
}
