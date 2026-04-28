using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public partial class AppUserLogin
    {
        public string UserId { get; set; } = null!;
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
    }
}
