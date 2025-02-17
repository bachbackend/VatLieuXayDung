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
    public class ReasonController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public ReasonController(B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllReason()
        {
            var reason = _context.Reasons.ToList();
            if (reason == null)
            {
                return NotFound("Không tìm thấy lý do nào.");
            }
            return Ok(reason);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAC([FromBody] ReasonRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reason = await _context.Reasons
                .FirstOrDefaultAsync(c => c.Name == dto.Name);

            if (reason != null)
            {
                return BadRequest("Lý do này đã tồn tại.");
            }

            var newReason = new Reason
            {
                Name = dto.Name,
            };

            _context.Reasons.Add(newReason);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReasonById), new { id = newReason.Id }, newReason);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetReasonById(int id)
        {
            var reason = await _context.Reasons.FindAsync(id);
            if (reason == null)
            {
                return NotFound("Lý do không tồn tại.");
            }
            return Ok(reason);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditReason(int id, [FromBody] ReasonRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingReason = await _context.Reasons.FindAsync(id);
            if (existingReason == null)
            {
                return NotFound("Lý do không tồn tại.");
            }

            var duplicateReason = await _context.Reasons
                .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);
            if (duplicateReason != null)
            {
                return BadRequest("Lý do đã tồn tại.");
            }

            existingReason.Name = dto.Name;

            _context.Reasons.Update(existingReason);
            await _context.SaveChangesAsync();

            return Ok(existingReason);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReason(int id)
        {
            var existingReason = await _context.Reasons.FindAsync(id);
            if (existingReason == null)
            {
                return NotFound(new { message = "Lý do không tồn tại." });
            }

            _context.Reasons.Remove(existingReason);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa lý do thành công." });
        }

    }
}
