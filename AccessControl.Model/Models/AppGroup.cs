using AccessControl.Model.Abtracts;
using System;
using System.Collections.Generic;

namespace AccessControl.Model.Models
{
    public class AppGroup : Auditable
    {
        public string? GroupCode { get; set; }
        public string? Name { get; set; }
    }
}
