using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Prize
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
