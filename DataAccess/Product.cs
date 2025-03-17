using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int? CertificateId { get; set; }

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;

    public sbyte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Description { get; set; } = null!;

    public int? SaleQuantity { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductCertificate> ProductCertificates { get; set; } = new List<ProductCertificate>();
}
