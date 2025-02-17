using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public sbyte Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastLogin { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
