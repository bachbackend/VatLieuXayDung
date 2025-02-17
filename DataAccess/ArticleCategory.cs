﻿using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class ArticleCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
