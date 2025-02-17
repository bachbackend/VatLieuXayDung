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
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public OrderController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }


        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder(
            int pageNumber = 1,
            int? pageSize = null,
            int? status = null,
            string? name = null
            //int? categoryId = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var orders = _context.Orders
                .Include(p => p.Status)
                .Include(p => p.Reason)
                .Include(p => p.User)
                .Include(p => p.ShippingAddress)
                    .ThenInclude(p => p.City)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                orders = orders.Where(p => p.User.Username.Contains(name));
            }

            //if (categoryId.HasValue)
            //{
            //    products = products.Where(p => p.CategoryId == categoryId.Value);
            //}

            if (status.HasValue)
            {
                orders = orders.Where(p => p.StatusId == status.Value);
            }

            orders = orders.OrderByDescending(p => p.OrderDate);

            int totalOrderCount = await orders.CountAsync();


            int totalPageCount = (int)Math.Ceiling(totalOrderCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<OrderDTO> orderWithPaging = await orders
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new OrderDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Username = p.User.Username,
                    CityId = p.ShippingAddress.CityId,
                    CityName = p.ShippingAddress.City.Name,
                    SpecificAddress = p.ShippingAddress.SpecificAddress,
                    PhoneNumber = p.ShippingAddress.PhoneNumber,
                    StatusId = p.StatusId,
                    StatusName = p.Status.Name,
                    ReasonId = p.ReasonId,
                    ReasonName = p.Reason.Name,
                    OrderDate = p.OrderDate,
                    TotalPrice = p.TotalPrice,
                    })
                    .ToListAsync();

            var result = new
            {
                Orders = orderWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }

        [HttpGet("GetOrderByOrderId/{orderId}")]
        public async Task<IActionResult> GetOrderByOrderId(int orderId)
        {
            var order = await _context.Orders
                .Include(p => p.Status)
                .Include(p => p.Reason)
                .Include(p => p.User)
                .Include(p => p.ShippingAddress)
                    .ThenInclude(p => p.City)
                .Where(p => p.Id == orderId)
                .Select(p => new OrderDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Username = p.User.Username,
                    CityId = p.ShippingAddress.CityId,
                    CityName = p.ShippingAddress.City.Name,
                    SpecificAddress = p.ShippingAddress.SpecificAddress,
                    PhoneNumber = p.ShippingAddress.PhoneNumber,
                    StatusId = p.StatusId,
                    StatusName = p.Status.Name,
                    ReasonId = p.ReasonId,
                    ReasonName = p.Reason.Name,
                    OrderDate = p.OrderDate,
                    TotalPrice = p.TotalPrice
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound(new { Message = "Không tìm thấy đơn hàng." });
            }

            return Ok(order);
        }

        [HttpGet("GetOrderByUserId/{userId}")]
        public async Task<IActionResult> GetOrderByUserId(
            int userId,
            int pageNumber = 1,
            int? pageSize = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var orders = _context.Orders
                .Include(p => p.Status)
                .Include(p => p.Reason)
                .Include(p => p.User)
                .Include(p => p.ShippingAddress)
                    .ThenInclude(p => p.City)
                .Where(p => p.UserId == userId)
                .AsQueryable();

            int totalOrderCount = await orders.CountAsync();

            int totalPageCount = (int)Math.Ceiling(totalOrderCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<OrderDTO> orderWithPaging = await orders
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new OrderDTO
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Username = p.User.Username,
                    SpecificAddress = p.ShippingAddress.SpecificAddress,
                    PhoneNumber = p.ShippingAddress.PhoneNumber,
                    CityId = p.ShippingAddress.CityId,
                    CityName = p.ShippingAddress.City.Name,
                    StatusId = p.StatusId,
                    StatusName = p.Status.Name,
                    ReasonId = p.ReasonId,
                    ReasonName = p.Reason.Name,
                    OrderDate = p.OrderDate,
                    TotalPrice = p.TotalPrice,
                })
                .ToListAsync();

            var result = new
            {
                Orders = orderWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }


        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromForm] int statusId)
        {
            // Kiểm tra nếu trạng thái là 6 (Hủy đơn hàng), không cho phép cập nhật
            if (statusId == 6)
            {
                return BadRequest(new { message = "Không được thực hiện hủy đơn hàng tại đây." });
            }

            // Kiểm tra sự hợp lệ của trạng thái
            var statusExists = await _context.OrderStatuses.AnyAsync(s => s.Id == statusId);
            if (!statusExists)
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            // Tìm đơn hàng theo orderId
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng." });
            }

            // Cập nhật trạng thái đơn hàng
            order.StatusId = statusId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Cập nhật trạng thái đơn hàng thành công!",
                    orderId = order.Id,
                    statusId = order.StatusId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi khi cập nhật: {ex.Message}" });
            }
        }


        [HttpPut("CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromForm] int reasonId)
        {
            // Kiểm tra lý do hủy
            var reasonExists = await _context.Reasons.AnyAsync(r => r.Id == reasonId);
            if (!reasonExists)
            {
                return Ok(new { message = "Lý do hủy đơn hàng không hợp lệ." });
            }

            // Tìm đơn hàng theo ID
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return Ok(new { message = "Không tìm thấy đơn hàng." });
            }

            // Kiểm tra trạng thái đơn hàng
            if (order.StatusId == 6)
            {
                return Ok(new { message = "Đơn hàng này đã bị hủy trước đó." });
            }

            if (order.StatusId == 4 || order.StatusId == 5)
            {
                return Ok(new { message = "Không thể hủy đơn hàng đang giao hoặc đã hoàn thành." });
            }

            // Cập nhật trạng thái
            order.StatusId = 6;
            order.ReasonId = reasonId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Đơn hàng đã được hủy thành công!",
                    orderId = order.Id,
                    statusId = order.StatusId,
                    reasonId = order.ReasonId
                });
            }
            catch (Exception ex)
            {
                return Ok(new { message = $"Đã xảy ra lỗi khi hủy đơn hàng: {ex.Message}" });
            }
        }


        [HttpPut("UpdateTotalPrice/{orderId}")]
        public async Task<IActionResult> UpdateTotalPrice(int orderId, [FromForm] decimal totalPrice)
        {
            // Kiểm tra giá trị totalPrice hợp lệ (không âm)
            if (totalPrice < 0)
            {
                return BadRequest("Tổng giá trị đơn hàng không thể là số âm.");
            }

            // Tìm đơn hàng theo orderId
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            // Cập nhật tổng giá trị đơn hàng
            order.TotalPrice = totalPrice;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Cập nhật tổng giá trị đơn hàng thành công!",
                    orderId = order.Id,
                    totalPrice = order.TotalPrice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi cập nhật tổng giá trị đơn hàng: {ex.Message}");
            }
        }


        [HttpGet("GetOrderDetailsByOrderId/{orderId}")]
        public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
        {
            var orders = await _context.Orders
                .Where(o => o.Id == orderId)
                .Include(p => p.Status)
                .Include(p => p.Reason)
                .Include(p => p.User)
                .Include(p => p.ShippingAddress)
                    .ThenInclude(p => p.City)
                .Include(p => p.OrderDetails)
                    .ThenInclude(p => p.Product)
                        .ThenInclude(p => p.Category)
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    Username = o.User.Username,
                    SpecificAddress = o.ShippingAddress.SpecificAddress,
                    PhoneNumber = o.ShippingAddress.PhoneNumber,
                    CityId = o.ShippingAddress.CityId,
                    CityName = o.ShippingAddress.City.Name,
                    StatusId = o.StatusId,
                    StatusName = o.Status.Name,
                    ReasonId = o.ReasonId,
                    ReasonName = o.Reason.Name,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,

                    // Thông tin chi tiết của OrderDetail
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                    {
                        Id = od.Id,
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        ProductName = od.Product.Name,
                        ProductImage = od.Product.Image,
                        Quantity = od.Quantity,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (orders == null)
            {
                return NotFound("Không tìm thấy chi tiết của đơn hàng.");
            }

            return Ok(orders);
        }

        //public class OrderDTO
        //{
        //    public int Id { get; set; }

        //    public int CustomerId { get; set; }

        //    public DateTime OrderDate { get; set; }

        //    public DateTime? ReceivedDate { get; set; }

        //    public int StatusId { get; set; }
        //    public string StatusName { get; set; } = null!;

        //    public int PaymentMethodId { get; set; }
        //    public string PaymentMethodName { get; set; } = null!;

        //    public decimal TotalPrice { get; set; }

        //    public decimal TotalRevenue { get; set; }
        //    public string UserName { get; set; } = null!;

        //    public string? Email { get; set; }

        //    public string PhoneNumber { get; set; } = null!;
        //    public bool IsPayment { get; set; }
        //    public int? ReasonId { get; set; }
        //    public string ReasonText { get; set; }
        //    public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
        //}
        



    }
}
