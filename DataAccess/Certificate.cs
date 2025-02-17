using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Certificate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Image { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ProductCertificate> ProductCertificates { get; set; } = new List<ProductCertificate>();
}
