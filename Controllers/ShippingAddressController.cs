using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VatLieuXayDung.DataAccess;
using VatLieuXayDung.Service;

namespace VatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public ShippingAddressController(B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllShippingAddress()
        {
            var sa = _context.ShippingAddresses.ToList();
            if (sa == null)
            {
                return NotFound("Không tìm thấy địa chỉ giao hàng nào.");
            }
            return Ok(sa);
        }

        [HttpGet("GetByUser/{userId}")]
        public async Task<IActionResult> GetShippingAddressByUser(int userId)
        {
            var shippingAddresses = _context.ShippingAddresses
                .Include(sa => sa.City)
                .Where(sa => sa.UserId == userId)
                .Select(sa => new
                {
                    sa.Id,
                    sa.UserId,
                    sa.CityId,
                    sa.City.Name,
                    sa.SpecificAddress,
                    sa.PhoneNumber
                })
                .ToList();

            if (!shippingAddresses.Any())
            {
                return NotFound("Không tìm thấy địa chỉ giao hàng nào cho người dùng này.");
            }

            return Ok(new
            {
                message = "Lấy danh sách địa chỉ giao hàng thành công.",
                shippingAddresses = shippingAddresses
            });
        }


    }
}
