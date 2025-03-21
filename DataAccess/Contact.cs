﻿using System;
using System.Collections.Generic;

namespace VatLieuXayDung.DataAccess;

public partial class Contact
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Status { get; set; }

    public DateTime ContactDate { get; set; }
}
