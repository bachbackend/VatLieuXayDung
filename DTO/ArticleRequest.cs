namespace VatLieuXayDung.DTO
{
    public class ArticleRequest
    {
        public int ArticleCategoryId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int UserId { get; set; }

        public sbyte Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ArticleRequestUpdate
    {
        public int ArticleCategoryId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int UserId { get; set; }

        public sbyte Status { get; set; }
    }
}
