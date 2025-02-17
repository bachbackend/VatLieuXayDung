using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Image
{
    public int Id { get; set; }

    public string Image1 { get; set; } = null!;

    public string Text { get; set; } = null!;
}
