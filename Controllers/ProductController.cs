using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using VatLieuXayDung.DataAccess;
using VatLieuXayDung.DTO;
using VatLieuXayDung.Service;
using static System.Net.Mime.MediaTypeNames;
using static VatLieuXayDung.DTO.ProductDTO;

namespace VatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings, IWebHostEnvironment environment)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
            _environment = environment;
        }

        [HttpGet("GetTop5NewestProducts")]
        public async Task<IActionResult> GetTop3NewestProducts()
        {
            // Lấy 3 sản phẩm mới nhất dựa trên CreatedAt
            var newestProducts = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToListAsync(); 

            return Ok(newestProducts);
        }

        [HttpGet("GetProductByCategoryId/{categoryId}")]
        public async Task<IActionResult> GetProductByCategoryId(int categoryId)
        {
            // Lấy danh sách sản phẩm thuộc danh mục, bao gồm thông tin danh mục và chứng nhận
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCertificates)
                .ThenInclude(pc => pc.Certificate)
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Certificates = p.ProductCertificates
                        .Select(pc => new CertificateDTO
                        {
                            Id = pc.Certificate.Id,
                            Name = pc.Certificate.Name
                        })
                    .ToList()
                })
                .ToListAsync();

            // Nếu không tìm thấy sản phẩm, trả về NotFound
            if (products == null || !products.Any())
            {
                return NotFound(new { Message = $"Không tìm thấy sản phẩm với danh mục Id = {categoryId}" });
            }

            // Trả về danh sách sản phẩm
            return Ok(products);
        }


        [HttpGet("GetProductsByCategoryIds")]
        public async Task<IActionResult> GetProductsByCategoryIds()
        {
            // Danh sách các CategoryId cần lọc
            var categoryIds = new List<int> { 3, 5 };

            // Lấy danh sách sản phẩm thuộc các danh mục 3 và 5, bao gồm thông tin danh mục và chứng nhận
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCertificates)
                .ThenInclude(pc => pc.Certificate)
                .Where(p => categoryIds.Contains(p.CategoryId))
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Certificates = p.ProductCertificates
                        .Select(pc => new CertificateDTO
                        {
                            Id = pc.Certificate.Id,
                            Name = pc.Certificate.Name
                        })
                    .ToList()
                })
                .ToListAsync();

            // Nếu không tìm thấy sản phẩm, trả về NotFound
            if (products == null || !products.Any())
            {
                return NotFound(new { Message = "Không tìm thấy sản phẩm trong các danh mục Id = 3 hoặc 5" });
            }

            // Trả về danh sách sản phẩm
            return Ok(products);
        }

        [HttpGet("GetRandomProducts")]
        public async Task<IActionResult> GetRandomProducts()
        {
            // Lấy danh sách sản phẩm từ cơ sở dữ liệu và sắp xếp ngẫu nhiên
            var randomProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCertificates)
                .ThenInclude(pc => pc.Certificate)
                .OrderBy(p => Guid.NewGuid())  // Sắp xếp ngẫu nhiên
                .Take(5)  // Lấy ra 5 sản phẩm
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Certificates = p.ProductCertificates
                        .Select(pc => new CertificateDTO
                        {
                            Id = pc.Certificate.Id,
                            Name = pc.Certificate.Name
                        })
                    .ToList()
                })
                .ToListAsync();

            // Nếu không tìm thấy sản phẩm, trả về NotFound
            if (randomProducts == null || !randomProducts.Any())
            {
                return NotFound(new { Message = "Không tìm thấy sản phẩm ngẫu nhiên" });
            }

            // Trả về danh sách sản phẩm ngẫu nhiên
            return Ok(randomProducts);
        }


        [HttpGet("GetRandomProducts8")]
        public async Task<IActionResult> GetRandomProducts8()
        {
            // Lấy danh sách sản phẩm từ cơ sở dữ liệu và sắp xếp ngẫu nhiên
            var randomProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCertificates)
                .ThenInclude(pc => pc.Certificate)
                .OrderBy(p => Guid.NewGuid())  // Sắp xếp ngẫu nhiên
                .Take(8)  // Lấy ra 5 sản phẩm
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Certificates = p.ProductCertificates
                        .Select(pc => new CertificateDTO
                        {
                            Id = pc.Certificate.Id,
                            Name = pc.Certificate.Name
                        })
                    .ToList()
                })
                .ToListAsync();

            // Nếu không tìm thấy sản phẩm, trả về NotFound
            if (randomProducts == null || !randomProducts.Any())
            {
                return NotFound(new { Message = "Không tìm thấy sản phẩm ngẫu nhiên" });
            }

            // Trả về danh sách sản phẩm ngẫu nhiên
            return Ok(randomProducts);
        }





        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct(
            int pageNumber = 1,
            int? pageSize = null,
            int? status = null,
            string? name = null,
            int? categoryId = null,
            string? sortBy = "id",
            string? sortOrder = "asc"
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCertificates)
                .ThenInclude(p => p.Certificate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                products = products.Where(p => p.Status == status.Value);
            }

            if (sortBy?.ToLower() == "name")
            {
                products = sortOrder.ToLower() == "desc"
                    ? products.OrderByDescending(p => p.Name)
                    : products.OrderBy(p => p.Name);
            }
            else if (sortBy?.ToLower() == "saleQuantity")
            {
                products = sortOrder.ToLower() == "desc"
                    ? products.OrderByDescending(p => p.SaleQuantity)
                    : products.OrderBy(p => p.SaleQuantity);
            }
            else if (sortBy?.ToLower() == "createDate")
            {
                products = sortOrder.ToLower() == "desc"
                    ? products.OrderByDescending(p => p.CreatedAt)
                    : products.OrderBy(p => p.CreatedAt);
            }
            else // Sắp xếp mặc định theo Id
            {
                products = sortOrder.ToLower() == "desc"
                    ? products.OrderByDescending(p => p.Id)
                    : products.OrderBy(p => p.Id);
            }

            int totalProductCount = await products.CountAsync();


            int totalPageCount = (int)Math.Ceiling(totalProductCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<ProductReturnDTO> productWithPaging = await products
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Certificates = p.ProductCertificates
                        .Select(pc => new CertificateDTO
                        {
                            Id = pc.Certificate.Id,
                            Name = pc.Certificate.Name
                        })
                    .ToList()
                })
            .ToListAsync();

            var result = new
            {
                Products = productWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }


        [HttpGet("GetAllProductStatusOne")]
        public async Task<IActionResult> GetAllProductStatusOne(
            int pageNumber = 1,
            int? pageSize = null,
            string? name = null,
            int? categoryId = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCertificates)
                .ThenInclude(p => p.Certificate)
                .Where(p => p.Status == 0)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }


            int totalProductCount = await products.CountAsync();


            int totalPageCount = (int)Math.Ceiling(totalProductCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<ProductReturnDTO> productWithPaging = await products
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Certificates = p.ProductCertificates
                        .Select(pc => new CertificateDTO
                        {
                            Id = pc.Certificate.Id,
                            Name = pc.Certificate.Name
                        })
                    .ToList()
                })
            .ToListAsync();

            var result = new
            {
                Products = productWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }


        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            // Tìm sản phẩm theo ID, bao gồm thông tin liên quan
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCertificates)
                .ThenInclude(pc => pc.Certificate)
                .Where(p => p.Id == id)
                .Select(p => new ProductReturnDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    Image = p.Image,
                    SaleQuantity = p.SaleQuantity,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Certificates = p.ProductCertificates
                        .Select(pc => new CertificateDTO
                        {
                            Id = pc.Certificate.Id,
                            Name = pc.Certificate.Name
                        })
                    .ToList()
                })
                .FirstOrDefaultAsync();

            // Nếu không tìm thấy sản phẩm, trả về lỗi 404
            if (product == null)
            {
                return NotFound(new { Message = "Product not found." });
            }

            // Trả về sản phẩm
            return Ok(product);
        }


        [HttpPost("addProduct")]
        public async Task<IActionResult> AddProduct(IFormFile file, [FromForm] ProductRequest model)
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
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo một sản phẩm mới và lưu vào database
            var product = new Product
            {
                CategoryId = model.CategoryId,
                CertificateId = model.CertificateId,  
                Name = model.Name,
                Image = fileName,  
                Description = model.Description,
                Status = model.Status,
                CreatedAt = DateTime.UtcNow,
                ProductCertificates = model.CertificateIds != null ? model.CertificateIds.Select(certificateId => new ProductCertificate { CertificateId = certificateId }).ToList() : new List<ProductCertificate>()
            };

            // Lưu sản phẩm vào database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { productId = product.Id, fileName = product.Image });
        }

        //[HttpPost("uploadImage")]
        //public async Task<IActionResult> UploadImage(IFormFile file, string altText)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file uploaded.");
        //    }

        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        //    var extension = Path.GetExtension(file.FileName).ToLower();
        //    if (!allowedExtensions.Contains(extension))
        //    {
        //        return BadRequest("Invalid file type.");
        //    }

        //    var fileName = Guid.NewGuid() + extension;
        //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    var image = new Models.Image
        //    {
        //        Image1 = fileName,
        //        Text = altText,
        //    };
        //    _context.Images.Add(image);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { imageId = image.Id, fileName = image.Image1 });
        //}

        [HttpGet("GetAllImages")]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _context.Images.ToListAsync();
            return Ok(images);
        }

        [HttpPut("updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductRequest model, IFormFile? file)
        {
            var product = await _context.Products.Include(p => p.ProductCertificates).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            product.CategoryId = model.CategoryId;
            product.Name = model.Name;
            product.Status = model.Status;
            product.Description = model.Description;

            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = "Invalid file type. Allowed types are .jpg, .jpeg, .png" });
                }

                var fileName = Guid.NewGuid() + extension;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                product.Image = fileName;
            }

            if (model.CertificateIds != null)
            {
                var existingProductCertificates = await _context.ProductCertificates
                    .Where(pc => pc.ProductId == id)
                    .ToListAsync();

                _context.ProductCertificates.RemoveRange(existingProductCertificates);

                product.ProductCertificates = model.CertificateIds.Select(certificateId => new ProductCertificate
                {
                    CertificateId = certificateId,
                    ProductId = id,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            await _context.SaveChangesAsync();

            // Trả về thông tin chi tiết sản phẩm đã cập nhật
            return Ok(new
            {
                message = "Product updated successfully.",
                productId = product.Id,
                name = product.Name,
                categoryId = product.CategoryId,
                description = product.Description,
                status = product.Status,
                image = product.Image
            });
        }


        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Tìm sản phẩm theo ID
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Xác định đường dẫn của ảnh để xóa
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", product.Image);

            // Kiểm tra và xóa ảnh nếu tồn tại
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // Xóa sản phẩm khỏi database
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Product deleted successfully.");
        }

        [HttpGet("GetCSVFile")]
        public async Task<ActionResult> GetProductCSVFile(int? productStatus)
        {
            string baseImageUrl = "https://localhost:7171/images/";

            var query = _context.Products
                .Include(od => od.Category)
                .AsQueryable();

            if (productStatus.HasValue)
            {
                query = query.Where(o => o.Status == productStatus.Value);
            }

            

            var products = await query.ToListAsync();

            StringBuilder strCSV = new StringBuilder();
            strCSV.AppendLine("STT,Tên sản phẩm,Ảnh sản phẩm,Loại sản phẩm,Trạng thái,Số lượng bán,Ngày tạo sản phẩm");

            int index = 1;
            foreach (var product in products)
            {
                string imageUrl = $"{baseImageUrl}{product.Image}";

                strCSV.AppendLine($"\"{index}\"," +
                                  $"\"{product.Name}\"," +
                                  $"\"{imageUrl}\"," +
                                  $"\"{product.Category.Name}\"," +
                                  $"\"{product.Status}\"," +
                                  $"\"{product.SaleQuantity}\"," +
                                  $"\"{product.CreatedAt}\"");
                index++;
            }

            byte[] bom = Encoding.UTF8.GetPreamble();
            using (MemoryStream memory = new MemoryStream())
            {
                memory.Write(bom, 0, bom.Length);
                using (StreamWriter writer = new StreamWriter(memory, Encoding.UTF8, 1024, leaveOpen: true))
                {
                    await writer.WriteAsync(strCSV.ToString());
                    await writer.FlushAsync();
                }
                memory.Position = 0;
                return File(memory.ToArray(), "text/csv", "Product.csv");
            }
        }



        //[HttpPost("uploadImage")]
        //public async Task<IActionResult> UploadImage(IFormFile file, string altText)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file uploaded.");
        //    }

        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        //    var extension = Path.GetExtension(file.FileName).ToLower();
        //    if (!allowedExtensions.Contains(extension))
        //    {
        //        return BadRequest("Invalid file type.");
        //    }

        //    var fileName = Guid.NewGuid() + extension;
        //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    var image = new Image
        //    {
        //        Image1 = fileName,
        //        AltText = altText,
        //    };
        //    _context.Images.Add(image);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { imageId = image.Id, fileName = image.Image1 });
        //}

        //[HttpGet("GetAllImages")]
        //public async Task<IActionResult> GetAllImages()
        //{
        //    var images = await _context.Images.ToListAsync();
        //    return Ok(images);
        //}

        //[HttpDelete("deleteImage/{id}")]
        //public async Task<IActionResult> DeleteImage(int id)
        //{
        //    var image = await _context.Images.FindAsync(id);
        //    if (image == null)
        //    {
        //        return NotFound("Image not found.");
        //    }

        //    _context.Images.Remove(image);
        //    await _context.SaveChangesAsync();

        //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", image.Image1);
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }

        //    return Ok("Image deleted successfully.");
        //}



        //[HttpGet("GetAllImages")]
        //public async Task<IActionResult> GetAllImages()
        //{
        //    var images = await _context.Images.ToListAsync();
        //    return Ok(images);
        //}

        //[HttpDelete("deleteImage/{id}")]
        //public async Task<IActionResult> DeleteImage(int id)
        //{
        //    var image = await _context.Images.FindAsync(id);
        //    if (image == null)
        //    {
        //        return NotFound("Image not found.");
        //    }

        //    _context.Images.Remove(image);
        //    await _context.SaveChangesAsync();

        //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", image.Image1);
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }

        //    return Ok("Image deleted successfully.");
        //}

    }
}
