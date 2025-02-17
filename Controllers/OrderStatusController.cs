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
    public class OrderStatusController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public OrderStatusController(B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllOrderStatus()
        {
            var os = _context.OrderStatuses.ToList();
            if (os == null)
            {
                return NotFound("Không tìm thấy trạng thái đơn hàng nào.");
            }
            return Ok(os);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddOrderStatus([FromBody] OsRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var os = await _context.OrderStatuses
                .FirstOrDefaultAsync(c => c.Name == dto.Name);

            if (os != null)
            {
                return BadRequest("Trạng thái đơn hàng này đã tồn tại.");
            }

            var newOs = new OrderStatus
            {
                Name = dto.Name,
            };

            _context.OrderStatuses.Add(newOs);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOsById), new { id = newOs.Id }, newOs);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetOsById(int id)
        {
            var os = await _context.OrderStatuses.FindAsync(id);
            if (os == null)
            {
                return NotFound("Trạng thái đơn hàng không tồn tại.");
            }
            return Ok(os);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditOs(int id, [FromBody] OsRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOs = await _context.OrderStatuses.FindAsync(id);
            if (existingOs == null)
            {
                return NotFound("Trạng thái đơn hàng không tồn tại.");
            }

            var duplicateOs = await _context.OrderStatuses
                .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);
            if (duplicateOs != null)
            {
                return BadRequest("Trạng thái đơn hàng đã tồn tại.");
            }

            existingOs.Name = dto.Name;

            _context.OrderStatuses.Update(existingOs);
            await _context.SaveChangesAsync();

            return Ok(existingOs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOs(int id)
        {
            var existingOs = await _context.OrderStatuses.FindAsync(id);
            if (existingOs == null)
            {
                return NotFound(new { message = "Trạng thái đơn hàng không tồn tại." });
            }

            _context.OrderStatuses.Remove(existingOs);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa trạng thái đơn hàng thành công." });
        }
    }
}
