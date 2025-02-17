﻿using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class ShippingAddress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CityId { get; set; }

    public string SpecificAddress { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
