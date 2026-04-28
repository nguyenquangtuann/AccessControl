using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class AppUserToken
    {
        public string UserId { get; set; } = null!;
        public string? LoginProvider { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
    }
}
