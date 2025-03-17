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
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;

        public CartController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings)
        {
            _configuration = configuration;
            _context = context;
            _paginationSettings = paginationSettings.Value;
        }

        [HttpGet("GetCartByUserId/{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng." });
            }

            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product) // Join với bảng Product để lấy thông tin sản phẩm
                .Select(c => new CartDTO
                {
                    UserId = userId,
                    Username = c.User.Username,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Image = c.Product.Image,
                    Quantity = c.Quantity
                })
                .ToListAsync();

            if (!cartItems.Any())
            {
                return Ok(new { message = "Giỏ hàng đang trống.", cartItems = new List<object>() });
            }


            return Ok(new
            {
                message = "Lấy danh sách sản phẩm trong giỏ hàng thành công.",
                cartItems = cartItems,
            });
        }

        [HttpGet("CountUniqueProducts/{userId}")]
        public async Task<IActionResult> CountUniqueProducts(int userId)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng." });
            }

            int uniqueProductCount = await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => c.ProductId)
                .Distinct()
                .CountAsync();

            return Ok(new
            {
                message = "Lấy tổng số sản phẩm khác nhau trong giỏ hàng thành công.",
                uniqueProductCount = uniqueProductCount
            });
        }



        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng." });
            }

            var cartItemsCount = await _context.Carts
                .CountAsync(x => x.UserId == customer.Id);

            if (cartItemsCount >= 20)
            {
                return BadRequest(new { message = "Giỏ hàng đã đạt tối đa 20 sản phẩm." });
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                return BadRequest(new { message = "Sản phẩm không tồn tại." });
            }

            if (quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng sản phẩm phải lớn hơn 0." });
            }

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == customer.Id && x.ProductId == productId);

            if (cartItem != null) // Nếu sản phẩm đã tồn tại trong giỏ hàng
            {
                int newQuantity = cartItem.Quantity + quantity;

                cartItem.Quantity = newQuantity;
            }
            else // Nếu sản phẩm chưa tồn tại trong giỏ hàng
            {

                var newCartItem = new Cart
                {
                    UserId = customer.Id,
                    ProductId = productId,
                    Quantity = quantity
                };

                await _context.Carts.AddAsync(newCartItem);
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm sản phẩm vào giỏ hàng thành công." });
        }


        [HttpPost("UpdateQuantityFromCart")]
        public async Task<IActionResult> UpdateQuantityFromCart(int userId, int productId, int quantity)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng." });
            }

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == customer.Id && x.ProductId == productId);

            if (cartItem == null)
            {
                return BadRequest(new { message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                return BadRequest(new { message = "Sản phẩm không tồn tại." });
            }

            if (quantity < 0)
            {
                return BadRequest(new { message = "Số lượng không được là số âm." });
            }

            if (quantity <= 0)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng." });
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            // Lấy danh sách sản phẩm trong giỏ hàng sau khi cập nhật
            var cartItems = await _context.Carts
                .Where(x => x.UserId == customer.Id)
                .Select(x => new
                {
                    x.ProductId,
                    x.Quantity
                })
                .ToListAsync();


            return Ok(new { message = "Cập nhật số lượng sản phẩm thành công.", cartItems = cartItems });
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int userId, int productId)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng." });
            }

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == customer.Id && x.ProductId == productId);

            if (cartItem == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sản phẩm đã được xóa khỏi giỏ hàng." });
        }

        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAll(int userId)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng." });
            }

            var cartItems = await _context.Carts
                .Where(x => x.UserId == customer.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return NotFound(new { message = "Giỏ hàng của khách hàng đang trống." });
            }

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tất cả sản phẩm đã được xóa khỏi giỏ hàng." });
        }

        //[HttpPost("PaymentCOD")]
        //public async Task<IActionResult> PaymentCOD(int userId)
        //{
        //    var customer = await _context.Users
        //        .FirstOrDefaultAsync(c => c.Id == userId);

        //    if (customer == null)
        //    {
        //        return NotFound("Không tìm thấy khách hàng.");
        //    }


        //    var cartItems = await _context.Carts
        //        .Where(x => x.UserId == customer.Id)
        //        .Include(x => x.Product)
        //        .ToListAsync();

        //    if (!cartItems.Any())
        //    {
        //        return BadRequest("Giỏ hàng trống hoặc không tìm thấy thông tin giỏ hàng.");
        //    }

        //    var order = new Order
        //    {
        //        UserId = customer.Id,
        //        OrderDate = DateTime.Now,
        //        StatusId = 1
        //    };

        //    await _context.Orders.AddAsync(order);
        //    await _context.SaveChangesAsync();

        //    foreach (var item in cartItems)
        //    {
        //        var orderDetail = new OrderDetail
        //        {
        //            OrderId = order.Id,
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity
        //        };

        //        // Cập nhật số lượng bán
        //        //item.ProductItem.Product.SaleQuantity += item.Quantity;

        //        await _context.OrderDetails.AddAsync(orderDetail);
        //    }

        //    await _context.SaveChangesAsync();

        //    // Xóa giỏ hàng sau khi thanh toán thành công
        //    _context.Carts.RemoveRange(cartItems);
        //    await _context.SaveChangesAsync();

        //    return Redirect($"http://127.0.0.1:5500/Home.html");
        //}


        //[HttpPost("PaymentCOD")]
        //public async Task<IActionResult> PaymentCOD([FromBody] PaymentRequestDTO request)
        //{
        //    var customer = await _context.Users
        //        .FirstOrDefaultAsync(c => c.Id == request.UserId);

        //    if (customer == null)
        //    {
        //        return NotFound("Không tìm thấy khách hàng.");
        //    }

        //    var cartItems = await _context.Carts
        //        .Where(x => x.UserId == customer.Id)
        //        .Include(x => x.Product)
        //        .ToListAsync();

        //    if (!cartItems.Any())
        //    {
        //        return BadRequest("Giỏ hàng trống hoặc không tìm thấy thông tin giỏ hàng.");
        //    }

        //    // Tạo đơn hàng mới
        //    var order = new Order
        //    {
        //        UserId = customer.Id,
        //        OrderDate = DateTime.Now,
        //        StatusId = 1 // Status "Đang xử lý" hoặc tương tự
        //    };

        //    await _context.Orders.AddAsync(order);
        //    await _context.SaveChangesAsync(); // Để lấy được order.Id

        //    // Xử lý địa chỉ giao hàng
        //    ShippingAddress shippingAddress;

        //    if (request.ShippingAddressId.HasValue)
        //    {
        //        // Nếu người dùng chọn địa chỉ đã có
        //        shippingAddress = await _context.ShippingAddresses
        //            .FirstOrDefaultAsync(sa => sa.Id == request.ShippingAddressId && sa.UserId == customer.Id);

        //        if (shippingAddress == null)
        //        {
        //            return BadRequest("Địa chỉ giao hàng không hợp lệ.");
        //        }
        //    }
        //    else if (request.NewShippingAddress != null)
        //    {
        //        // Nếu người dùng nhập địa chỉ mới
        //        shippingAddress = new ShippingAddress
        //        {
        //            OrderId = order.Id,
        //            UserId = customer.Id,
        //            CityId = request.NewShippingAddress.CityId,
        //            SpecificAddress = request.NewShippingAddress.SpecificAddress,
        //            PhoneNumber = request.NewShippingAddress.PhoneNumber
        //        };

        //        await _context.ShippingAddresses.AddAsync(shippingAddress);
        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        return BadRequest("Vui lòng chọn hoặc nhập địa chỉ giao hàng.");
        //    }

        //    // Thêm chi tiết đơn hàng
        //    foreach (var item in cartItems)
        //    {
        //        var orderDetail = new OrderDetail
        //        {
        //            OrderId = order.Id,
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity
        //        };

        //        await _context.OrderDetails.AddAsync(orderDetail);
        //    }

        //    await _context.SaveChangesAsync();

        //    // Xóa giỏ hàng sau khi thanh toán thành công
        //    _context.Carts.RemoveRange(cartItems);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Thanh toán thành công!", orderId = order.Id });
        //}

        [HttpPost("PaymentCOD")]
        public async Task<IActionResult> PaymentCOD([FromBody] PaymentRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.UserId);
            if (customer == null)
                return NotFound("Không tìm thấy khách hàng.");

            var cartItems = await _context.Carts
                .Where(x => x.UserId == customer.Id)
                .Include(x => x.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Giỏ hàng trống hoặc không tìm thấy thông tin giỏ hàng.");

            // Xử lý địa chỉ giao hàng
            ShippingAddress shippingAddress;

            if (request.ShippingAddressId.HasValue)
            {
                shippingAddress = await _context.ShippingAddresses
                    .FirstOrDefaultAsync(sa => sa.Id == request.ShippingAddressId && sa.UserId == customer.Id);

                if (shippingAddress == null)
                    return BadRequest("Địa chỉ giao hàng không hợp lệ.");
            }
            else if (request.NewShippingAddress != null)
            {
                shippingAddress = new ShippingAddress
                {
                    UserId = customer.Id,
                    CityId = request.NewShippingAddress.CityId,
                    SpecificAddress = request.NewShippingAddress.SpecificAddress,
                    PhoneNumber = request.NewShippingAddress.PhoneNumber
                };

                await _context.ShippingAddresses.AddAsync(shippingAddress);
                await _context.SaveChangesAsync(); // Lưu để có shippingAddress.Id
            }
            else
            {
                return BadRequest("Vui lòng chọn hoặc nhập địa chỉ giao hàng.");
            }

            // Tạo đơn hàng sau khi địa chỉ hợp lệ
            var order = new Order
            {
                UserId = customer.Id,
                ShippingAddressId = shippingAddress.Id,  // Gán địa chỉ vào đơn hàng
                OrderDate = DateTime.Now,
                StatusId = 1, // Đang xử lý
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(); // Lưu để có order.Id

            // Thêm chi tiết đơn hàng
            var orderDetails = cartItems.Select(item => new OrderDetail
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            await _context.OrderDetails.AddRangeAsync(orderDetails);

            // Cập nhật số lượng bán của sản phẩm
            foreach (var item in cartItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product != null)
                {
                    product.SaleQuantity = (product.SaleQuantity ?? 0) + item.Quantity;
                }
            }

            // Xóa giỏ hàng và lưu tất cả thay đổi
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thanh toán thành công!", orderId = order.Id });
        }






    }
}
