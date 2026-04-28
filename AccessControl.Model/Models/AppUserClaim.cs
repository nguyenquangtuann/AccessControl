using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class AppUserClaim
    {
        public string UserId { get; set; } = null!;
        public int Id { get; set; }
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }
    }
}
