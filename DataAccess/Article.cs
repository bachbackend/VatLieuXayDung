using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Article
{
    public int Id { get; set; }

    public int ArticleCategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Image { get; set; } = null!;

    public int UserId { get; set; }

    public sbyte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ArticleCategory ArticleCategory { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
