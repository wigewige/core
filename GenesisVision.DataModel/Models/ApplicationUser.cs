﻿using System;
using Microsoft.AspNetCore.Identity;

namespace GenesisVision.DataModel.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public AspNetUsers AspNetUsers { get; set; }
    }
}
