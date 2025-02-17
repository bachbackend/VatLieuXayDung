using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class ProductCertificate
{
    public int ProductId { get; set; }

    public int CertificateId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Certificate Certificate { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
