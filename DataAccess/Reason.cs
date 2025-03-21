﻿using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Reason
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
