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
    public class ArticleController : ControllerBase
    {
        private readonly B3ifu0huowhy6xqzhw41Context _context;
        private readonly PaginationSettings _paginationSettings;
        private readonly IWebHostEnvironment _environment;

        public ArticleController(IConfiguration configuration, B3ifu0huowhy6xqzhw41Context context, IOptions<PaginationSettings> paginationSettings, IWebHostEnvironment environment)
        {
            _context = context;
            _paginationSettings = paginationSettings.Value;
            _environment = environment;
        }

        [HttpGet("GetAllArticle")]
        public async Task<IActionResult> GetAllArticle(
            int pageNumber = 1,
            int? pageSize = null,
            int? status = null,
            string? title = null,
            int? categoryId = null
            )
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var articles = _context.Articles
                .Include(p => p.ArticleCategory)
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                articles = articles.Where(p => p.Title.Contains(title));
            }

            if (categoryId.HasValue)
            {
                articles = articles.Where(p => p.ArticleCategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                articles = articles.Where(p => p.Status == status.Value);
            }

            int totalArticleCount = await articles.CountAsync();


            int totalPageCount = (int)Math.Ceiling(totalArticleCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<ArticleReturnDTO> articletWithPaging = await articles
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new ArticleReturnDTO
                {
                    Id = p.Id,
                    ArticleCategoryId = p.ArticleCategoryId,
                    ArticleCategoryName = p.ArticleCategory.Name,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User.Username,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
            .ToListAsync();

            var result = new
            {
                Articles = articletWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }

        [HttpGet("GetRandom6Article")]
        public async Task<IActionResult> GetRandom6Article()
        {
            var randomArticles = await _context.Articles
                .Include(p => p.ArticleCategory)
                .Include(p => p.User)
                .OrderBy(r => Guid.NewGuid()) // Lấy ngẫu nhiên
                .Take(6)
                .Select(p => new ArticleReturnDTO
                {
                    Id = p.Id,
                    ArticleCategoryId = p.ArticleCategoryId,
                    ArticleCategoryName = p.ArticleCategory.Name,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User.Username,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(randomArticles);
        }

        [HttpGet("GetRandom5Article")]
        public async Task<IActionResult> GetRandom5Article()
        {
            var randomArticles = await _context.Articles
                .Include(p => p.ArticleCategory)
                .Include(p => p.User)
                .OrderBy(r => Guid.NewGuid()) // Lấy ngẫu nhiên
                .Take(5)
                .Select(p => new ArticleReturnDTO
                {
                    Id = p.Id,
                    ArticleCategoryId = p.ArticleCategoryId,
                    ArticleCategoryName = p.ArticleCategory.Name,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User.Username,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(randomArticles);
        }


        [HttpGet("GetAllArticleStatusZero")]
        public async Task<IActionResult> GetAllArticleStatusZero(
    int pageNumber = 1,
    int? pageSize = null,
    string? title = null,
    int? categoryId = null
)
        {
            int actualPageSize = pageSize ?? _paginationSettings.DefaultPageSize;
            var articles = _context.Articles
                .Include(p => p.ArticleCategory)
                .Include(p => p.User)
                .Where(p => p.Status == 0)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                articles = articles.Where(p => p.Title.Contains(title));
            }

            if (categoryId.HasValue)
            {
                articles = articles.Where(p => p.ArticleCategoryId == categoryId.Value);
            }


            // Sắp xếp theo CreatedAt giảm dần
            articles = articles.OrderByDescending(p => p.CreatedAt);

            int totalArticleCount = await articles.CountAsync();

            int totalPageCount = (int)Math.Ceiling(totalArticleCount / (double)actualPageSize);
            int nextPage = pageNumber + 1 > totalPageCount ? pageNumber : pageNumber + 1;
            int previousPage = pageNumber - 1 < 1 ? pageNumber : pageNumber - 1;

            var pagingResult = new PagingReturn
            {
                TotalPageCount = totalPageCount,
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };

            List<ArticleReturnDTO> articletWithPaging = await articles
                .Skip((pageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => new ArticleReturnDTO
                {
                    Id = p.Id,
                    ArticleCategoryId = p.ArticleCategoryId,
                    ArticleCategoryName = p.ArticleCategory.Name,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User.Username,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
            .ToListAsync();

            var result = new
            {
                Articles = articletWithPaging,
                Paging = pagingResult
            };

            return Ok(result);
        }


        [HttpGet("GetArticleById/{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            // Tìm bài viết theo Id, bao gồm thông tin danh mục và người dùng
            var article = await _context.Articles
                .Include(p => p.ArticleCategory)
                .Include(p => p.User)
                .Where(p => p.Id == id)
                .Select(p => new ArticleReturnDTO
                {
                    Id = p.Id,
                    ArticleCategoryId = p.ArticleCategoryId,
                    ArticleCategoryName = p.ArticleCategory.Name,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User.Username,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
                .FirstOrDefaultAsync();

            // Nếu không tìm thấy bài viết, trả về NotFound
            if (article == null)
            {
                return NotFound(new { Message = $"Không tìm thấy bài viết với Id = {id}" });
            }

            // Trả về bài viết
            return Ok(article);
        }

        [HttpGet("GetArticleByArticleCategoryId/{categoryId}")]
        public async Task<IActionResult> GetArticleByArticleCategoryId(int categoryId)
        {
            // Tìm tất cả bài viết thuộc danh mục, bao gồm thông tin danh mục và người dùng
            var articles = await _context.Articles
                .Include(p => p.ArticleCategory)
                .Include(p => p.User)
                .Where(p => p.ArticleCategoryId == categoryId)
                .Select(p => new ArticleReturnDTO
                {
                    Id = p.Id,
                    ArticleCategoryId = p.ArticleCategoryId,
                    ArticleCategoryName = p.ArticleCategory.Name,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User.Username,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            // Nếu không tìm thấy bài viết, trả về NotFound
            if (articles == null || !articles.Any())
            {
                return NotFound(new { Message = $"Không tìm thấy bài viết với danh mục Id = {categoryId}" });
            }

            // Trả về danh sách bài viết
            return Ok(articles);
        }

        [HttpGet("GetLatestArticles")]
        public async Task<IActionResult> GetLatestArticles()
        {
            // Lấy 3 bài viết mới nhất, sắp xếp theo CreatedAt giảm dần
            var latestArticles = await _context.Articles
                .Include(p => p.ArticleCategory)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)  // Sắp xếp theo thời gian tạo bài viết giảm dần
                .Take(3)  // Lấy 3 bài viết đầu tiên
                .Select(p => new ArticleReturnDTO
                {
                    Id = p.Id,
                    ArticleCategoryId = p.ArticleCategoryId,
                    ArticleCategoryName = p.ArticleCategory.Name,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    UserId = p.UserId,
                    UserName = p.User.Username,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            // Nếu không có bài viết nào, trả về NotFound
            if (latestArticles == null || !latestArticles.Any())
            {
                return NotFound(new { Message = "Không có bài viết nào." });
            }

            // Trả về 3 bài viết mới nhất
            return Ok(latestArticles);
        }


        [HttpPost("addArticle")]
        public async Task<IActionResult> AddArticle(IFormFile file, [FromForm] ArticleRequest model)
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
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/article", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo một sản phẩm mới và lưu vào database
            var article = new Article
            {
                ArticleCategoryId = model.ArticleCategoryId,
                Title = model.Title,
                Content = model.Content,
                Image = fileName,
                UserId = 1,
                Status = model.Status,
                CreatedAt = DateTime.UtcNow
            };

            // Lưu sản phẩm vào database
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return Ok(new { articleId = article.Id, fileName = article.Image });
        }

        [HttpPut("updateArticle/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ArticleRequestUpdate model, IFormFile? file)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(p => p.Id == id);
            if (article == null)
            {
                return NotFound("Article not found.");
            }

            article.ArticleCategoryId = model.ArticleCategoryId;
            article.Title = model.Title;
            article.Content = model.Content;
            article.UserId = model.UserId;
            article.Status = model.Status;

            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file type.");
                }

                var fileName = Guid.NewGuid() + extension;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/article", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                article.Image = fileName;
            }


            await _context.SaveChangesAsync();

            return Ok(new { message = "Article updated successfully." });
        }

        [HttpDelete("deleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            // Tìm bài viết theo ID
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound("Article not found.");
            }

            // Xác định đường dẫn ảnh để xóa
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/article", article.Image);

            // Kiểm tra và xóa ảnh nếu tồn tại
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // Xóa bài viết khỏi database
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok("Article deleted successfully.");
        }

    }
}
