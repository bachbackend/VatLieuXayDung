namespace VatLieuXayDung.DTO
{
    public class ArticleReturnDTO
    {
        public int Id { get; set; }

        public int ArticleCategoryId { get; set; }
        public string ArticleCategoryName { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Image { get; set; } = null!;

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;

        public sbyte Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
