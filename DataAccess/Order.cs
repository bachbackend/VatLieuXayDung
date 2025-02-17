using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public int? ReasonId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public int ShippingAddressId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Reason? Reason { get; set; }

    public virtual ShippingAddress ShippingAddress { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
